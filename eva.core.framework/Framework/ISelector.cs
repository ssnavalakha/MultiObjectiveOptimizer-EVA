using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eva.core.framework.framework
{
    /// <summary>
    /// interface for selectors
    /// </summary>
    public interface ISelector
    {
        /// <summary>
        /// Generic method to be implemented for selection
        /// </summary>
        /// <param name="sm">the solution manager to work with </param>
        /// <returns>The selected solutions</returns>
        IEnumerable<ISolution> select(ISolutionManger sm);
    }
    /// <summary>
    /// Abstract class for selectors
    /// </summary>
    public abstract class Selectors : ISelector
    {
        /// <summary>
        /// method to select solutions
        /// </summary>
        /// <param name="sm">the solution manager to work with</param>
        /// <returns>The selected solutions</returns>
        public abstract IEnumerable<ISolution> select(ISolutionManger sm);

    }
}
