using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eva.core.framework.framework;

namespace eva.example.mill.mill
{
    /// <summary>
    /// Objective to evalate Hp Late orders
    /// </summary>
    public class ObjectiveLateHighPriorityOrders : Objective
    {
        /// <summary>
        /// Constructor taking parameters from config file
        /// </summary>
        /// <param name="para">the parameters read from a config file</param>
        public ObjectiveLateHighPriorityOrders(Dictionary<String, Object> para) : base(para)
        {

        }

        /// <summary>
        /// default constructor it assigns name and wether
        /// this property is to maximized or minimized
        /// </summary>
        public ObjectiveLateHighPriorityOrders()
        {
            name = "HPLateOrder";
            isAsc = true;
        }

        /// <summary>
        /// iterates through assigned orders and calculates high priority late orders
        /// accross all machines
        /// </summary>
        /// <param name="solution">the solution to evaluate</param>
        /// <returns>the evaluation result</returns>
        public override double evaluate(ISolution solution)
        {
            var sln = (ProductionSchedule)solution;
            double noOfLateOrders = 0;
            sln.getProposedOrder().ForEach(ordering =>
            {
                int timeTaken = 0;
                int previousPaperGrade = -1;
                ordering.ForEach(order =>
                {
                    if (order.productCode != previousPaperGrade && previousPaperGrade != -1)
                        timeTaken = timeTaken + ((MillProductionResources)sln.getResources()).milConfig.setup_time;
                    if (timeTaken + order.productionCycles > order.dueDate && order.highPriority)
                        noOfLateOrders++;
                    timeTaken = timeTaken + order.productionCycles;
                    previousPaperGrade = order.productCode;
                });
            });
            return noOfLateOrders;
        }
    }
}
