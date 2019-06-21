using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eva.core.framework.framework
{
    /// <summary>
    /// The default solution selector
    /// </summary>
    public class DefaultSolutionSelector : Selectors
    {
        /// <summary>
        /// Selects no solutions
        /// </summary>
        /// <param name="sm">The solution manager to work with</param>
        /// <returns>an empty list</returns>
        public override IEnumerable<ISolution> select(ISolutionManger sm)
        {
            return new List<ISolution>();
        }
    }
}
