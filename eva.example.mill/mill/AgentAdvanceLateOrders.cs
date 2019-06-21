using System.Collections.Generic;
using eva.core.framework.framework;

namespace eva.example.mill.mill
{
    /// <summary>
    /// Worker agent which advances late orders
    /// </summary>
    public class AgentAdvanceLateOrders : WorkerAgent
    {
        /// <summary>
        /// Every agent has to have an activator and a selector
        /// </summary>
        /// <param name="act">the activator checks if the agent has to be activated</param>
        /// <param name="sct">the selector selects solutions on which the agent runs</param>
        public AgentAdvanceLateOrders(IAgentActivator act, ISelector sct) : base(act, sct)
        {
        }
        /// <summary>
        /// Finds and Advances every late order by one position 
        /// </summary>
        /// <param name="selected">the selected solutions</param>
        /// <param name="sm">the solution manager to work with</param>
        /// <returns>list of newly created solutions</returns>
        protected override IEnumerable<ISolution> runWithSolutions(IEnumerable<ISolution> selected,ISolutionManger sm)
        {
            var result = new List<ISolution>();
            using (var enumerator = selected.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    var ps = (ProductionSchedule)enumerator.Current;
                    var proposedOrders = ps.getProposedOrder();

                    //iterate on each machines current schedule
                    proposedOrders.ForEach(assignedMachine =>
                    {
                        int timeTaken = 0;
                        Order prevOrder = null;
                        for  (int i=0;i<assignedMachine.Count;i++)
                        {
                            if (prevOrder!=null && assignedMachine[i].productCode != prevOrder.productCode)
                                timeTaken = timeTaken + ((MillProductionResources) ps.getResources()).milConfig
                                            .setup_time;
                            if (timeTaken + assignedMachine[i].productionCycles > assignedMachine[i].dueDate)
                            {
                               if (i != 0)
                                {
                                    timeTaken = timeTaken + assignedMachine[i].productionCycles;
                                    prevOrder = assignedMachine[i-1];

                                    assignedMachine[i - 1] = assignedMachine[i];
                                    assignedMachine[i] = prevOrder;
                                    continue;
                                }
                            }
                            timeTaken = timeTaken + assignedMachine[i].productionCycles;
                            prevOrder = assignedMachine[i];
                        }
                    });
                    result.Add(ps);
                }
            }
            return result;
        }
    }
}
