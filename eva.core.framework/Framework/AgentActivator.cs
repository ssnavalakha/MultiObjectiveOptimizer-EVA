using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eva.core.framework.framework
{
    /// <summary>
    /// An interface for Agent Activators
    /// </summary>
    public interface IAgentActivator
    {
        /// <summary>
        /// Test for activation
        /// </summary>
        /// <param name="sm">The solution manager to be used for testing</param>
        /// <returns>the result of test</returns>
        bool activate(ISolutionManger sm);
    }

    /// <summary>
    /// An Abstract class for all Agent Activators
    /// </summary>
    public abstract class AgentActivator : IAgentActivator
    {
        /// <summary>
        /// Test for activation
        /// </summary>
        /// <param name="sm">The solution manager to be used for testing</param>
        /// <returns>the result of test</returns>
        public abstract bool activate(ISolutionManger sm);
    }
    
    /// <summary>
    /// an agent that always returns true for CreatorAgents 
    /// </summary>
    public class DefaultAgentActivator : AgentActivator
    {
        /// <summary>
        /// always true
        /// </summary>
        /// <param name="sm">The solution manager to be used for testing</param>
        /// <returns>the result of test</returns>
        public override bool activate(ISolutionManger sm)
        {
            return true;
        }
    }
    
    /// <summary>
    /// an agent which works where there are one or more solutions present in the population
    /// </summary>
    public class NonZeroPopulationActivator : AgentActivator
    {
        /// <summary>
        /// returns true only when there is a solution in the population
        /// </summary>
        /// <param name="sm">The solution manager to be used for testing</param>
        /// <returns>the result of test</returns>
        public override bool activate(ISolutionManger sm)
        {
            return sm.population.Any();
        }
    }
    
    /// <summary>
    /// an agent that only works when there are certain number of solutions present in the population
    /// </summary>
    public class PopulationSizeRangeActivator: AgentActivator
    {
        /// <summary>
        /// private member storing the min number
        /// </summary>
        private int minimum;
        /// <summary>
        /// Constructor taking the input for minimum number of solutions
        /// </summary>
        /// <param name="min">the number</param>
        public PopulationSizeRangeActivator(int min)
        {
            minimum = min;
        }
        /// <summary>
        /// activates only after a certain number of solutions are presnet in the population
        /// </summary>
        /// <param name="sm">The solution manager to be used for testing</param>
        /// <returns>the result of test</returns>
        public override bool activate(ISolutionManger sm)
        {
            return sm.population.Count()>minimum;
        }
    }
}
