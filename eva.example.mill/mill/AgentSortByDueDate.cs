using System.Collections.Generic;
using eva.core.framework.framework;

namespace eva.example.mill.mill
{
    /// <summary>
    /// Sorts order by due date
    /// </summary>
    public class AgentSortByDueDate : WorkerAgent
    {
        /// <summary>
        /// Every agent has to have an activator and a selector
        /// </summary>
        /// <param name="act">the activator checks if the agent has to be activated</param>
        /// <param name="sct">the selector selects solutions on which the agent runs</param>s
        public AgentSortByDueDate(IAgentActivator act, ISelector sct) : base(act, sct)
        {
        }
        /// <summary>
        /// Sorts the orders on each machine by their due date.
        /// </summary>
        /// <param name="selected">the solution selected</param>
        /// <param name="sm">the solution manager to work with</param>
        /// <returns>the newly generated solution after sorting orders </returns>
        protected override IEnumerable<ISolution> runWithSolutions(IEnumerable<ISolution> selected,ISolutionManger sm)
        {
            var result = new List<ISolution>();
            using (var enumerator = selected.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    var ps = (ProductionSchedule)enumerator.Current;
                    var proposedOrders = ps.getProposedOrder();

                    proposedOrders.ForEach(assignedMachine => assignedMachine.Sort((x,y) => x.dueDate.CompareTo(y.dueDate)));
                    result.Add(ps);
                }
            }
            return result;
        }
    }
}
