﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace eva.core.framework.framework
{
    // selector selects activator activates
    // run is left abstract
    /// <summary>
    /// Abstract class for all Types of Agents
    /// public members -
    ///                 IAgentActivator activator
    ///                 ISelector selector
    /// protected members -
    ///                 batchid - the current batch being processed
    ///                 SolutionIdGenerator generateId - a way to generate ids before inserting in a database
    /// </summary>
    public abstract class Agent : IAgent
    {
        /// <summary>
        /// Batch Id of the solutions this agent will work on
        /// </summary>
        protected int batchid { get; set; }
        /// <summary>
        /// This agents activator
        /// </summary>
        public IAgentActivator activator { get; set; }
        /// <summary>
        /// This agents selector
        /// </summary>
        public ISelector selector { get; set; }
        /// <summary>
        /// This agents solution id generator
        /// </summary>
        protected SolutionIdGenerator generateId { get; set; }

        private BlockingCollection<ISolution> databaseStream;

        /// <summary>
        /// inserts solutions generated by the agent into a database
        /// </summary>
        /// <param name="slns">the solutions to be inserted</param>
        /// <returns>The soutions after inserting them into the database</returns>
        public virtual IEnumerable<ISolution> insertIntoDatabase(IEnumerable<ISolution> slns)
        {
            return slns.Select(x =>
            {
                databaseStream.Add(x);
                return x;
            });
        }
        /// <summary>
        /// Runs the agent using the solution manager
        /// </summary>
        /// <param name="sm">the solution manager to be used while running the agent</param>
        /// <returns>the solutions generated by the agent</returns>
        public abstract IEnumerable<ISolution> run(ISolutionManger sm);

        /// <summary>
        /// Every agent has to have an activator and a selector
        /// </summary>
        /// <param name="act">the activator checks if the agent has to be activated</param>
        /// <param name="sct">the selector selects solutions on which the agent runs</param>
        protected Agent(IAgentActivator act, ISelector sct)
        {
            activator = act;
            selector = sct;
        }
        /// <summary>
        /// tells you wether the agent has to be activated
        /// </summary>
        /// <param name="sm">the solution manager as a required field to run the test</param>
        /// <returns>returns the result of the test</returns>
        public bool activate(ISolutionManger sm)
        {
            return activator.activate(sm);
        }

        /// <summary>
        /// Assigns a batchid on which the solution has to work
        /// </summary>
        /// <param name="batch_id">the batch id to be assigned</param>
        public void assignBatchId(int batch_id)
        {
            this.batchid = batch_id;
        }

        /// <summary>
        /// Assigns a solutionId Generator to the agent which can later be used to generate solutions
        /// a new instace of the generator is created everyt time for thread safety
        /// </summary>
        /// <param name="gn"> the generator to be used</param>
        public void assignGenerator(SolutionIdGenerator gn)
        {
            // Creates a new Solution Generator Object for thread safety and assigns it once
            // to the agent
            if (this.generateId == null)
            {
                var genType = gn.GetType();
                var parameters = new Object[3];
                parameters[0] = "all_solutions";
                parameters[1] = 100;
                parameters[2] = gn.dbapi;
                var newGenerator=Activator.CreateInstance(genType,parameters); 
                generateId = (SolutionIdGenerator)newGenerator;
            }
        }

        /// <summary>
        /// Takes in a blocking collection and assigns to this agent
        /// </summary>
        /// <param name="stream">the blocking collection to be assigned</param>
        public void assignStream(BlockingCollection<ISolution> stream)
        {
            if (databaseStream == null)
            {
                databaseStream = stream;
            }
        }

        /// <summary>
        /// Selects the solutions on which the agent will run 
        /// </summary>
        /// <param name="sm">the solution manager which is used to select the solutions</param>
        /// <returns>selectd solutions</returns>
        public IEnumerable<ISolution> select(ISolutionManger sm)
        {
            return selector.select(sm);
        }
    }
}
