﻿using System.Collections.Generic;
using System.Linq;

namespace eva.core.framework.framework
{
    /// <summary>
    /// abstract class for creater agents
    /// </summary>
    public abstract class CreaterAgent : Agent
    {
        /// <summary>
        /// Creates new solutions
        /// </summary>
        /// <returns>Returns the newly created solutions</returns>
        protected abstract IEnumerable<ISolution> createNewSolutions();
        /// <summary>
        /// Every agent has to have an activator and a selector
        /// </summary>
        /// <param name="act">the activator checks if the agent has to be activated</param>
        /// <param name="sct">the selector selects solutions on which the agent runs</param>
        protected CreaterAgent(IAgentActivator act, ISelector sct) : base(act, sct)
        {
        }

        /// <summary>
        /// First checks if the solution is activated and then creates new solution
        /// then inserts the newly created solutions into the database after assinging
        /// them a batch id and solution id else returns an empty solution
        /// </summary>
        /// <param name="sm">the solution manager to be used while running the agent</param>
        /// <returns>the solutions generated by the agent</returns>
        public override IEnumerable<ISolution> run(ISolutionManger sm)
        {
            // check if the solution is active
            if (activate(sm))
            {
                //create new solutions and then insert them into the database
                // after assigning them an id and bacth id
                return insertIntoDatabase(createNewSolutions().Select(x =>
                {
                    x.batchid = batchid;
                    x.solutionid = generateId.getId();
                    return x;
                }));
            }
            else
                return new List<ISolution>();
        }

    }
}