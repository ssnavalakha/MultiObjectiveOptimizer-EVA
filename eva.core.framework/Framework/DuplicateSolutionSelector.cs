using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eva.core.framework.framework
{
    /// <summary>
    /// Returns unique Solutions
    /// </summary>
    public class DuplicateSolutionSelector : Selectors
    {
        /// <summary>
        /// selects unique solutions from the solution population
        /// </summary>
        /// <param name="sm">the solution manager to work with </param>
        /// <returns>the unique solutions</returns>
        public override IEnumerable<ISolution> select(ISolutionManger sm)
        {
            // A simple distinct solves our problem this uses the .equals and .hascode method
            // please make sure they are performant 
            return sm.population.Distinct();

        }
    }
}
