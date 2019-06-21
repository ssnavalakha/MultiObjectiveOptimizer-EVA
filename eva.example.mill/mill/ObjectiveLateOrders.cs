using System;
using System.Collections.Generic;
using eva.core.framework.framework;

namespace eva.example.mill.mill
{
    /// <summary>
    /// Objective to represent the number of late orders
    /// </summary>
    public class ObjectiveLateOrders : Objective
    {
        /// <summary>
        /// Constructor taking parameters from config file
        /// </summary>
        /// <param name="para">the parameters read from a config file</param>
        public ObjectiveLateOrders(Dictionary<String, Object> para) : base(para)
        {

        }

        /// <summary>
        /// default constructor it assigns name and wether
        /// this property is to maximized or minimized
        /// </summary>
        public ObjectiveLateOrders()
        {
            name = "LateOrder";
            isAsc = true;
        }

        /// <summary>
        /// Evaluates the number of late orders
        /// </summary>
        /// <param name="solution">the solution to evaluate</param>
        /// <returns>the number of late orders in this solution</returns>
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
                    if (timeTaken + order.productionCycles > order.dueDate)
                        noOfLateOrders++;
                    timeTaken = timeTaken + order.productionCycles;
                    previousPaperGrade = order.productCode;
                });
            });
            return noOfLateOrders;
        }
    }
}
