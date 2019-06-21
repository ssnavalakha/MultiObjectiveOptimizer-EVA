using System;
using System.Collections.Generic;
using System.Linq;

namespace eva.core.framework.framework
{
    /// <summary>
    /// Selects the non-dominated solutions
    /// </summary>
    public class NonDominatedSolutionSelector : Selectors
    {
        /// <summary>
        ///  The algorithm used is specified in this paper
        /// http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.206.4467&rep=rep1&type=pdf
        ///O => result
        ///    S1 => nonDominatedslns
        ///For every solution Oi
        ///    (other than first solution )
        ///of list O, compare solution Oi from the solutions
        ///of S1
        ///i. If any element of set S1 dominate Oi
        ///    ,
        ///    Delete Oi
        ///    from the list
        ///ii. If Oi
        ///dominate any solution of the set
        ///S1, Delete that solution from S1
        ///    iii. If Oi
        ///is non dominated to set S1, Then
        ///    update set S1 = S1 U Oi
        ///iv. If set S1 becomes empty add immediate
        ///    solution at immediate solution to S1 
        /// </summary>
        /// <param name="sm">the solution manager to work with</param>
        /// <returns>A list of Non dominated solutions</returns>
        public override IEnumerable<ISolution> select(ISolutionManger sm)
        {
            IOrderedEnumerable<ISolution> slns;
            // sort the solution based on the first objective and wethr maximizing or minimizing 
            // that objective is advantageous
            if(sm.objs[0].isAsc)
                slns=sm.population.OrderBy(x => x.objectiveEvaluation[0].evaluationResult);
            else
                slns=sm.population.OrderByDescending(x => x.objectiveEvaluation[0].evaluationResult);

            var nonDominatedslns = new List<ISolution>();
            bool isFirstsln = true;
            // iterate over all solutions
            foreach(var sln in slns)
            {
                //if its the first solution add it to the list of non-dominated solutions
                // as we have sorted the solutions hence the first solution has to be in the list of non-dominated solutions
                if (isFirstsln)
                {
                    isFirstsln = false;
                    nonDominatedslns.Add(sln);
                    continue;
                }
                //if the list of non dominated solutions becomes empty at any point 
                // pick the next available solution
                if (!nonDominatedslns.Any())
                {
                    nonDominatedslns.Add(sln);
                    continue;
                }

                // this code block checks if any solution in the non-dominated
                // list of solutions dominates this particular solution if yes 
                // set the flag to true and move to the new solution
                var continueFlag = false;
                for(int i=0;i<nonDominatedslns.Count;i++)
                {
                    if (checkifSlnDominates(nonDominatedslns[i], sln, sm))
                    {
                        continueFlag = true;
                        break;
                    }
                }

                if(continueFlag)
                    continue;
                
                // find all the solutions in the non-dominated solution list
                // which this particular solution dominates and delete them from the list of non-dominated solutions
                // as we have found a better solution
                for (int i=nonDominatedslns.Count-1;i>=0;i--)
                {
                    if (checkifSlnDominates(sln, nonDominatedslns[i], sm))
                        nonDominatedslns.RemoveAt(i);
                }

                // if this solution does not dominate any non dominated solution list which we have currently found
                // and no solution in our non -dominated list of solutions dominates this guy
                // add it to the list else forget it
                bool flag = false;
                foreach (var nonDsln in nonDominatedslns)
                {
                    if (checkifSlnDominates(nonDsln, sln, sm) || checkifSlnDominates(sln, nonDsln, sm))
                    {
                        flag = true;
                        break;
                    }
                }
                if(!flag)
                    nonDominatedslns.Add(sln);
            }
            return nonDominatedslns;
        }

        /// <summary>
        /// Tests if solutions s1 dominates s2
        /// by comparing each objective evaluation
        /// </summary>
        /// <param name="s1">First solution</param>
        /// <param name="s2">Second Solution</param>
        /// <param name="sm">The solution Manager to work with</param>
        /// <returns>the result of the test</returns>
        private bool checkifSlnDominates(ISolution s1, ISolution s2,ISolutionManger sm)
        {
            var result = new List<bool>();
            // depending on what the objective is find if s1 dominates s2
            // by doing simple equality checks
            for(int i=0;i<sm.objs.Count;i++)
            {
                if (sm.objs[i].isAsc && s1.objectiveEvaluation[i].evaluationResult < s2.objectiveEvaluation[i].evaluationResult)
                {
                    result.Add(true);
                    continue;
                }
                if (!sm.objs[i].isAsc && s1.objectiveEvaluation[i].evaluationResult > s2.objectiveEvaluation[i].evaluationResult)
                {
                    result.Add(true);
                    continue;
                }
                if(s1.objectiveEvaluation[i].evaluationResult == s2.objectiveEvaluation[i].evaluationResult)
                    continue;
                result.Add(false);
            }

            if (result.Count==0 ||result.Any(x => x == false))
                return false;
            return true;
        }
    }
}
