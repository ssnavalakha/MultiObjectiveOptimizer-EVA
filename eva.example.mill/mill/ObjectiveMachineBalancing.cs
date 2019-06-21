using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eva.core.framework.framework;

namespace eva.example.mill.mill
{
    /// <summary>
    /// Objective representing machine balancing
    /// </summary>
    public class ObjectiveMachineBalancing : Objective
    {
        /// <summary>
        /// Constructor taking parameters from config file
        /// </summary>
        /// <param name="para">the parameters read from a config file</param>s
        public ObjectiveMachineBalancing(Dictionary<String, Object> para) : base(para)
        {

        }
        /// <summary>
        /// default constructor it assigns name and wether
        /// this property is to maximized or minimized
        /// </summary>
        public ObjectiveMachineBalancing()
        {
            name = "MachineBalance";
            isAsc = true;
        }
        /// <summary>
        /// evaluates how effectively the machines are balanced
        /// </summary>
        /// <param name="solution">the solution to evaluate</param>
        /// <returns>the evaluation result</returns>
        public override double evaluate(ISolution solution)
        {
            var sln = (ProductionSchedule)solution;
            var proposedOrder = sln.getProposedOrder();
            var totalMachines = proposedOrder.Count;
            var totalJobs = proposedOrder.Sum(x => x.Count);
            double bestBalance = Math.Round((double)totalJobs/totalMachines,2);
            double result = proposedOrder.Sum(x =>Math.Round( Math.Abs((double)bestBalance -x.Count ),2));
            return result;
        }
    }
}
