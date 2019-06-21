using System.Collections.Generic;
using System.Linq;

namespace eva.core.framework.framework
{
    /// <summary>
    /// An Abstract class for Destroyer Agents
    /// </summary>
    public abstract class DestroyerAgent : Agent
    {
        /// <summary>
        /// The solution Manager to wor with
        /// </summary>
        protected ISolutionManger slnManager;

        /// <summary>
        /// Generic method which destroies Solutions and inserts the solutions
        /// destrouyed in the database (works in reverse order)
        /// </summary>
        /// <param name="solutions">the solutions not be destroyed</param>
        public virtual void destroy(IEnumerable<ISolution> solutions)
        {
            var slnList = solutions.ToList(); //converts the solution into a list
            // adds the deleted solution into the database by checking if they exist in the solutions we got 
            var deletedSolutions = insertIntoDatabase(slnManager.population
                .Where(solution=>!slnList.Any(x=>x.solutionid==solution.solutionid))).ToList();
        
            slnManager.population = slnList;
        }

        /// <summary>
        /// Constructor to create destroyer agents
        /// </summary>
        /// <param name="sm">the solution manager to work with</param>
        /// <param name="act">the activator checks if the agent has to be activated</param>
        /// <param name="sct">the selector selects solutions on which the agent runs</param>
        protected DestroyerAgent(ISolutionManger sm, IAgentActivator act, ISelector sct) :base (act,sct)
        {
            slnManager = sm;
        }
        ///// <summary>
        ///// inserts deleted solutions into the database
        ///// </summary>
        ///// <param name="solutions">The solutions to be inserted into the database</param>
        ///// <returns>the same solutions but they are now in the deleted_solutions Table</returns>
        //public override IEnumerable<ISolution>insertIntoDatabase(IEnumerable<ISolution> solutions)
        //{
        //    var dpApi = new DbApi();
        //    return dpApi.insertIntoDeletedSolutions(solutions);
        //}

        /// <summary>
        /// runs the agent after checking if its activated by selecting solutions not to be deleted
        /// </summary>
        /// <param name="sm">the solution manager to work with</param>
        /// <returns>An empty list of solutions</returns>
        public override IEnumerable<ISolution> run(ISolutionManger sm)
        {
            // if activated destroy hehehehehawhawhaw
            if(activate(sm))
                destroy(select(sm));
            return new List<ISolution>();
        }
    }

    /// <summary>
    /// nothing special selector does the main work 
    /// </summary>
    public class RemoveDuplicates : DestroyerAgent
    {
        /// <summary>
        /// Constructor to create destroyer agents
        /// </summary>
        /// <param name="sm">the solution manager to work with</param>
        /// <param name="act">the activator checks if the agent has to be activated</param>
        /// <param name="sct">the selector selects solutions on which the agent runs</param>
        public RemoveDuplicates(ISolutionManger sm, IAgentActivator act, ISelector sct) :base(sm,act,sct)
        {
        }

        
        /// <summary>
        /// Wethod which destroies Solutions and inserts the solutions
        /// destrouyed in the database (works in reverse order)
        /// </summary>
        /// <param name="solutions">the solutions not be destroyed</param>
        public override void destroy(IEnumerable<ISolution> solutions)
        {
            base.destroy(solutions);
        }
        
    }

    /// <summary>
    /// nothing special selector does the main work 
    /// </summary>
    public class UserEnabledDestroyer : DestroyerAgent
    {
        /// <summary>
        /// Constructor to create destroyer agents
        /// </summary>
        /// <param name="sm">the solution manager to work with</param>
        /// <param name="act">the activator checks if the agent has to be activated</param>
        /// <param name="sct">the selector selects solutions on which the agent runs</param>
        public UserEnabledDestroyer(ISolutionManger sm, IAgentActivator act, ISelector sct) :base(sm,act,sct)
        {
        }

        /// <summary>
        /// Wethod which destroies Solutions and inserts the solutions
        /// destrouyed in the database (works in reverse order)
        /// </summary>
        /// <param name="solutions">the solutions not be destroyed</param>
        public override void destroy(IEnumerable<ISolution> solutions)
        {
            base.destroy(solutions);
        }
        
    }
    
    /// <summary>
    /// nothing special selector does the main work 
    /// </summary>
    public class RemoveDominatedSolutions : DestroyerAgent
    {
        /// <summary>
        /// Constructor to create destroyer agents
        /// </summary>
        /// <param name="sm">the solution manager to work with</param>
        /// <param name="act">the activator checks if the agent has to be activated</param>
        /// <param name="sct">the selector selects solutions on which the agent runs</param>
        public RemoveDominatedSolutions(ISolutionManger sm, IAgentActivator act, ISelector sct) : base(sm, act, sct)
        {
        }

        /// <summary>
        /// Wethod which destroies Solutions and inserts the solutions
        /// destrouyed in the database (works in reverse order)
        /// </summary>
        /// <param name="solutions">the solutions not be destroyed</param>
        public override void destroy(IEnumerable<ISolution> solutions)
        {
            base.destroy(solutions);
        }
    }
}
