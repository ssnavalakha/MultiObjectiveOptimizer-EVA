using System.Collections.Generic;
using eva.core.framework.framework;

namespace eva.example.mill.mill
{
    /// <summary>
    /// Picks an order and moves it to the end of any machine
    /// could also be the same machine
    /// </summary>
    public class AgentMoveRandomOrder : WorkerAgent
    {
        /// <summary>
        /// Every agent has to have an activator and a selector
        /// </summary>
        /// <param name="act">the activator checks if the agent has to be activated</param>
        /// <param name="sct">the selector selects solutions on which the agent runs</param>s
        public AgentMoveRandomOrder(IAgentActivator act, ISelector sct) : base(act, sct)
        {
        }

        /// <summary>
        /// Picks a random order from any machine and moves it to the end of a randomly selected machine 
        /// (possibly the machine it is currently assigned to).</summary>
        /// <param name="selected">the solution selected</param>
        /// <param name="sm">the solution manager to work with</param>
        /// <returns>the newly generated solution after order change</returns>
        protected override IEnumerable<ISolution> runWithSolutions(IEnumerable<ISolution> selected,ISolutionManger sm)
        {
            var result = new List<ISolution>();
            using (var enumerator = selected.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    var ps = (ProductionSchedule)enumerator.Current;
                    var millResources = (MillProductionResources) ps.getResources();
                    var proposedOrderSchedule = ps.getProposedOrder();
                    var rnadomListNumberGenerator = new SecureRandom();
                    int randomListNumber = rnadomListNumberGenerator.Next(millResources.milConfig.machines);
                    var randomOrderNumberGenerator = new SecureRandom();
                    int randomOrderNumber = randomOrderNumberGenerator.Next(proposedOrderSchedule[randomListNumber].Count);


                    if (proposedOrderSchedule[randomListNumber].Count != 0)
                    {
                        var order = proposedOrderSchedule[randomListNumber][randomOrderNumber];
                        proposedOrderSchedule[randomListNumber].RemoveAt(randomOrderNumber);
                        randomListNumber = rnadomListNumberGenerator.Next(millResources.milConfig.machines);
                        proposedOrderSchedule[randomListNumber].Add(order);
                       
                    }
                    ps.setProposedList(proposedOrderSchedule);
                    result.Add(ps);
                }
            }
            return result;
        }
    }
}
