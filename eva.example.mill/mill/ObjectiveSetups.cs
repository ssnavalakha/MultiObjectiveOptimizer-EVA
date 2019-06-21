using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eva.core.framework.framework;

namespace eva.example.mill.mill
{
    /// <summary>
    /// Objective representing total setup time
    /// </summary>
   public class ObjectiveSetups : Objective
    {
        /// <summary>
        /// Constructor taking parameters from config file
        /// </summary>
        /// <param name="para">the parameters read from a config file</param>
        public ObjectiveSetups(Dictionary<String, Object> para) : base(para)
        {

        }
        /// <summary>
        /// default constructor it assigns name and wether
        /// this property is to maximized or minimized
        /// </summary>
        public ObjectiveSetups()
        {
            name = "Setup";
            isAsc = true;
        }
        
        /// <summary>
        /// iterates through assigned orders and calculates setup time
        /// accross all machines
        /// </summary>
        /// <param name="solution">the solution to evaluate</param>
        /// <returns>the evaluation result</returns>
        public override double evaluate(ISolution solution)
        {
            var sln = (ProductionSchedule)solution;
            double setupCosts = 0;
            sln.getProposedOrder().ForEach(ordering =>
            {
                for (int i = 0; i < ordering.Count - 1; i++)
                    if (ordering[i].productCode != ordering[i + 1].productCode)
                        setupCosts = setupCosts + ((MillProductionResources)sln.getResources()).milConfig.setup_time;
            });
            return setupCosts;
        }
    }
}
