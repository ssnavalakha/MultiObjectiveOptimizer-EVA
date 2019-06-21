using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eva.core.framework.framework
{
    /// <summary>
    /// An abstract class for all solution managers contains objectives
    /// solution population various agents and resources
    /// </summary>
     public abstract class SolutionManager : ISolutionManger
     {
         /// <summary>
         /// A way to retrieve connection making framework independant of where the connection is stored)
         /// </summary>
        public IConnectionRetriever ConnectionRetriever { get; set; }
         /// <summary>
         /// The database api to interact with
         /// </summary>
         public Func<IDbApi> DbApi { get; set; }

         /// <summary>
         /// The objectives of to optimize
         /// </summary>
        public List<Objective> objs { get; set; }
         /// <summary>
         /// The solutions produced by the optimizer
         /// </summary>
        public IEnumerable<ISolution> population { get; set; }
         /// <summary>
         /// the batch id to work with
         /// </summary>
        public int batchid { get; set; }
         /// <summary>
         /// Creator agents for creating solutions
         /// </summary>
        public List<CreaterAgent> createrAgents { get; set; }
         /// <summary>
         /// The destroyer agents to destroy the solutions
         /// </summary>
        public List<DestroyerAgent> destroyerAgents { get; set; }
         /// <summary>
         /// the worker agents to work on the solutions
         /// </summary>
        public List<WorkerAgent> workerAgents{ get; set; }
         /// <summary>
         /// The solution id generator for agents
         /// </summary>
        public SolutionIdGenerator gen { get; set; }
         /// <summary>
         /// the single static copy of the resources
         /// </summary>
        private static Resource availableResources { get; set; }

         /// <summary>
         /// A thread safe collection to insert solutions into all solutions
         /// </summary>
         private BlockingCollection<ISolution> allSolutionsStream;

         /// <summary>
         /// A thread safe collection to insert solutions into deleted solutions
         /// </summary>
         private BlockingCollection<ISolution> deletedSolutionsStream;

        public abstract void printStuff();

         /// <summary>
         /// constructor
         /// </summary>
         /// <param name="connectionRetriever">the retriever from where to fetch the connection</param>
         protected SolutionManager(IConnectionRetriever connectionRetriever)
         {
             ConnectionRetriever = connectionRetriever;
         }
         /// <summary>
         /// find a better way please
         /// </summary>
         /// <param name="r">The Resources</param>
         public static void SetResource(Resource r)
         {
             availableResources = r;
         }

         /// <summary>
         /// find a better way please
         /// </summary>
         /// <returns></returns>
         public static Resource GetResource()
         {
             return availableResources.Copy();
         }

         /// <summary>
         /// concats two lists to return a new list
         /// </summary>
         /// <param name="slns">the solution list to be combined with the population</param>
        public void addSolutions(IEnumerable<ISolution> slns)
        {
            // evaluates the solution against each of the objects defined 
            // and stores it in the object
            var slnList = slns.ToList();
            if(slnList.Any())
            {
                // simple concatination for performance
                population = population.Concat(slnList.Select(x =>
                {
                    runAndEvaluateObjective(x);
                    return x;
                })).ToList();
            }
        }
        /// <summary>
        /// fetches the next available batchid and returns it
        /// </summary>
        /// <returns>the next available batch id</returns>
        public int fetchBatchId()
        {
            // call the database
            return DbApi().getBatchId();
        }

         /// <summary>
         /// runs the optimizer
         /// </summary>
        public void run()
         {
             initializeStreams();
            // assign a batch id for the solution manager
            batchid = fetchBatchId();
            // assign a solution id generator for the manager
            gen = new SolutionIdGenerator("all_solutions",100,DbApi());
            
             var task1 = startAllSolutionInsert();
             var task2 = startDeletedSolutionInsert();
            // run the agents and clean up
            for(int i=0;i<300;i++)
                runAgents(createrAgents,allSolutionsStream);
            
            for (int i = 0; i < 12; i++)
            {
                runAgents(workerAgents,allSolutionsStream);
                if(i!=0 && i%3==0)
                   runAgents(destroyerAgents,deletedSolutionsStream);
            }
            cleanup();
            stopStreams();
            waitForThreads(task1, task2);
         }

         /// <summary>
         /// waits for the assigned threads
         /// </summary>
         /// <param name="t1">first thread</param>
         /// <param name="t2">second thread</param>
         private void waitForThreads(Task t1, Task t2)
         {
             var taskList=new List<Task>();
             taskList.Add(t1);
             taskList.Add(t2);
             Task.WaitAll(taskList.ToArray());
         }

         /// <summary>
         /// Stops the streams from taking in any new elements
         /// </summary>
         private void stopStreams()
         {
             allSolutionsStream.CompleteAdding();
             deletedSolutionsStream.CompleteAdding();
         }

         /// <summary>
         /// initilaizes the streams
         /// </summary>
         private void initializeStreams()
         {
             allSolutionsStream = new BlockingCollection<ISolution>();
             deletedSolutionsStream =new BlockingCollection<ISolution>();
         }

         /// <summary>
         /// async task which starts inserting into all solutions
         /// </summary>
         /// <returns>the task object</returns>
         private Task startAllSolutionInsert()
         {
             return Task.Factory.StartNew(() =>
             {
                 DbApi().insertIntoAllSolutions(allSolutionsStream.GetConsumingEnumerable());
             });
         }

         /// <summary>
         /// async task which starts inserting into deleted solutions
         /// </summary>
         /// <returns>the task object</returns>
         private Task startDeletedSolutionInsert()
         {
             return Task.Factory.StartNew(() =>
             {
                 DbApi().insertIntoDeletedSolutions(deletedSolutionsStream.GetConsumingEnumerable());
             });
         }
         /// <summary>
         /// Runs all the destroyer agents as a clean up
         /// </summary>
        public virtual void cleanup()
        {
            // create new destroyer agents
            // for clean up purposes only and run them
            var cleanupTasjStep1 = new RemoveDuplicates(this, new NonZeroPopulationActivator(),
                new DuplicateSolutionSelector());
            cleanupTasjStep1.assignBatchId(batchid);
            cleanupTasjStep1.assignStream(deletedSolutionsStream);
            cleanupTasjStep1.run(this);
            var cleanupTaskStep2 = new RemoveDominatedSolutions(this, new NonZeroPopulationActivator(), 
                new NonDominatedSolutionSelector());
            cleanupTaskStep2.assignBatchId(batchid);
            cleanupTaskStep2.assignStream(deletedSolutionsStream);
            cleanupTaskStep2.run(this);

        }

         /// <summary>
         /// evaluates objectives based on the solution
         /// </summary>
         /// <param name="x">the solution to run evaluate objectives against</param>
         private void runAndEvaluateObjective(ISolution x)
         {
             // evaluate each objective and store it
             for (int i = 0; i < objs.Count; i++)
             {
                 if(x.objectiveEvaluation.Count<objs.Count)
                     x.objectiveEvaluation.Add(new EvaluationResult(objs[i].name, objs[i].evaluate(x)));
                 else
                     x.objectiveEvaluation[i] = new EvaluationResult(objs[i].name,objs[i].evaluate(x));
             }
         }
        
         /// <summary>
         /// Re runs the optimizer using the defined rules 
         /// </summary>
         /// <param name="newRules">the list of new rules</param>
         /// <param name="batchId">the batchid of the solutions againast which this runs</param>
        public void rerun(List<Rule> newRules,int batchId)
        {
            this.batchid = batchId;
            // fetch solutions from database and add them to our population
            initializeStreams();
            this.population = getSolutionsForABatchId(batchId).Select(x =>
            {
                runAndEvaluateObjective(x);
                return x;
            });
            //assign the solution id generator
            gen = new SolutionIdGenerator("all_solutions",100,DbApi());
            createRulesAndDestroyerAgents(newRules); // create destroyer agents to incorporate rules
            var task1 = startAllSolutionInsert();
            var task2 = startDeletedSolutionInsert();
            for (int i = 0; i < 12; i++)
            {
                runAgents(workerAgents,allSolutionsStream);
                if(i!=0 && i%3==0 )
                    runAgents(destroyerAgents,deletedSolutionsStream);
            }
            // clean up
            cleanup();
            stopStreams();
            waitForThreads(task1, task2);
        }
        /// <summary>
        /// creates new destroyer agents based on the rules
        /// </summary>
        /// <param name="newRules">the rules</param>
        private void createRulesAndDestroyerAgents(List<Rule> newRules)
        {
            // create rule expressions
            var ruleExprs = RulesExpressionGenerator.CreateRuleExpression<EvaluationResult>(newRules);
            foreach (var ruleExpr in ruleExprs)
            {
                destroyerAgents.Add(new UserEnabledDestroyer(this,
                    new NonZeroPopulationActivator(), new UserEnabledSelectors(ruleExpr.Value,ruleExpr.Key)));
            }
        }

         /// <summary>
         /// an abstract method which tells the solution manager on how to get the solutions for a particular
         /// batch id
         /// </summary>
         /// <param name="batchId"> the batch id</param>
         /// <returns>the list of fetched solutions</returns>
        protected abstract IEnumerable<ISolution> getSolutionsForABatchId(int batchId);
        
        /// <summary>
        /// runs the agents
        /// </summary>
        /// <param name="agents">the agents to run</param>
        /// <param name="stream">the stream with which the agents would work with</param>
         private void runAgents(IEnumerable<IAgent> agents,BlockingCollection<ISolution> stream)
        {
            // assign batch id and generator to agents and run them
            foreach (var ag in agents)
            {
                ag.assignBatchId(batchid);
                ag.assignGenerator(gen);
                ag.assignStream(stream);
                addSolutions(ag.run(this));
            }
        }
    }
}