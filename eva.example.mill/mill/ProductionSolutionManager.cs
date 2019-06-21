using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using eva.core.framework.framework;
using Newtonsoft.Json;

namespace eva.example.mill.mill
{
    /// <summary>
    /// Mill production Solution manager
    /// </summary>
    public class ProductionSolutionManager : SolutionManager
    {
        /// <summary>
        /// Sets up the Solution Manager for this solution
        /// </summary>
        /// <param name="connectionRetriever">the retriever to fetch connections from</param>
        public ProductionSolutionManager(IConnectionRetriever connectionRetriever) 
            : base(connectionRetriever)
        {
            objs = new List<Objective>();
            objs.Add(new ObjectiveSetups());
            objs.Add(new ObjectiveLateOrders());
            objs.Add(new ObjectiveLateHighPriorityOrders());
            objs.Add(new ObjectiveMachineBalancing());
            population = new ConcurrentBag<ISolution>();
            createrAgents = new List<CreaterAgent>();
            workerAgents = new List<WorkerAgent>();
            destroyerAgents = new List<DestroyerAgent>();
            createrAgents.Add(new AgentAssignOrdersRandomly(new DefaultAgentActivator(), new DefaultSolutionSelector()));
            workerAgents.Add(new AgentMoveRandomOrder(new NonZeroPopulationActivator(), new RandomSolutionSelector()));
            workerAgents.Add(new AgentAdvanceLateOrders(new NonZeroPopulationActivator(), new RandomSolutionSelector()));
            workerAgents.Add(new AgentSortByDueDate(new NonZeroPopulationActivator(), new RandomSolutionSelector()));
            destroyerAgents.Add(new RemoveDuplicates(this, new NonZeroPopulationActivator(), new DuplicateSolutionSelector()));
            destroyerAgents.Add(new RemoveDominatedSolutions(this, new NonZeroPopulationActivator(), new NonDominatedSolutionSelector()));
            DbApi = () => new DbApi(connectionRetriever);
        }



        /// <summary>
        /// method used to print stuff on the console
        /// </summary>
        public override void printStuff()
        {
            Console.WriteLine("Total number of solutions " + population.Count());
            foreach(var pop in population)
            {
                Console.WriteLine("-------------------------------------------------------------------");
                var proposedOrder = ((ProductionSchedule)pop).getProposedOrder();
                int count = proposedOrder.Count;
                Console.WriteLine(pop.objectiveEvaluation[0].evaluationResult+"    "+pop.objectiveEvaluation[1].evaluationResult
                                  +"    "+pop.objectiveEvaluation[2].evaluationResult
                                  +"    "+pop.objectiveEvaluation[3].evaluationResult);
                for (int i = 0; i < count; i++)
                {
                    Console.WriteLine("Machine "+ i);
                    proposedOrder[i].ForEach(order =>
                    {
                        Console.WriteLine(order.orderId +"    "+ order.productCode + "    " + order.dueDate
                                          + "    " + order.productionCycles + "    " + order.highPriority);
                    });
                }
            }
        }

        /// <summary>
        /// a method which tells us how to fetch solutions for a particular batch id
        /// </summary>
        /// <param name="batchId">the batch id of the solutions to be fetched</param>
        /// <returns>the list of non dominated solutions</returns>
        protected override IEnumerable<ISolution> getSolutionsForABatchId(int batchId)
        {
            var dpapi=new DbApi(ConnectionRetriever);
            return dpapi.getSolutionsForABatchId(batchId, (serializedData) =>
                {
                    return JsonConvert.DeserializeObject<ProductionSchedule>(serializedData);
                });
        }
    }
}