using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eva.core.framework.framework
{
    /// <summary>
    /// Represents a solution manager
    /// </summary>
    public interface ISolutionManger
    {
        /// <summary>
        /// method to run the optimizer
        /// </summary>
        void run();
        /// <summary>
        /// Temporary method to print stuff on the screen
        /// </summary>
        void printStuff();
        /// <summary>
        /// the list of objectives against which a solution is to be evaluated
        /// </summary>
        List<Objective> objs { get; set; }
        /// <summary>
        /// current solutionwith which the optimizer is working with 
        /// </summary>
        IEnumerable<ISolution> population{ get; set; }
        /// <summary>
        /// creator agents to create solutions
        /// </summary>
        List<CreaterAgent> createrAgents { get; set; }
        /// <summary>
        /// destroyer agents
        /// </summary>
        List<DestroyerAgent> destroyerAgents { get; set; }
        /// <summary>
        /// worer agents
        /// </summary>
        List<WorkerAgent> workerAgents{ get; set; }
        /// <summary>
        /// the batch id of this run
        /// </summary>
        int batchid { get; set; }
        /// <summary>
        /// method which re runs the opotimizer using rules
        /// </summary>
        /// <param name="newRules">The Rules</param>
        /// <param name="batchId">Batchid of the previous run</param>
        void rerun(List<Rule> newRules,int batchId);
        /// <summary>
        /// the solution id generator
        /// </summary>
        SolutionIdGenerator gen { get; set; }

        /// <summary>
        /// A way to retrieve connection making framework independant of where the connection is stored)
        /// </summary>
        IConnectionRetriever ConnectionRetriever { get; set; }

        /// <summary>
        /// The database api to interact with
        /// </summary>
        Func<IDbApi> DbApi { get; set; }
    }
}
