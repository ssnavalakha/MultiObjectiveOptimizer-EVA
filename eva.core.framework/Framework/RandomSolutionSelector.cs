using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eva.core.framework.framework
{
    /// <summary>
    /// Randomly selects a solution from the population
    /// </summary>
    public class RandomSolutionSelector : Selectors
    {
        /// <summary>
        /// Selects a solution at random using Secure random a non-pseudo random number generator
        /// selects a soltuion clones it and returns it
        /// </summary>
        /// <param name="sm">the solution manger to work with </param>
        /// <returns>a list of the new solution</returns>
        public override IEnumerable<ISolution> select(ISolutionManger sm)
        {
            var rnd = new SecureRandom(); // create an object of a non-psedo random number generator
            int r = rnd.Next(sm.population.Count()); // find the next random number
            var sln = sm.population.ElementAt(r); // fetch the random solution
            var randomSln = sln.doClone(); // do a clone of the fetched object
            var result = new List<ISolution>();
            result.Add(randomSln);
            return result; // wrap the clonned object in a list and return
        }
    }
}
