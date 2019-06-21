using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace eva.core.framework.framework
{
    /// <summary>
    /// this selector is used to select solutions using rules
    /// </summary>
    public class UserEnabledSelectors : Selectors
    {
        /// <summary>
        /// the rule expression which takes in an evaluation result and tells us if the solution satisfies
        /// the rule defined by the user
        /// </summary>
        private Expression<Func<EvaluationResult, bool>> ruleExpr;
        /// <summary>
        /// the name of the objective on which the rule has run
        /// </summary>
        private string _objectiveName;

        /// <summary>
        /// Constructor to run rule destroyer agents
        /// </summary>
        /// <param name="ruleExpr">the code to run the rule</param>
        /// <param name="objName">the objective to run the rule against</param>
        public UserEnabledSelectors(Expression<Func<EvaluationResult,bool>> ruleExpr,String objName)
        {
            this.ruleExpr = ruleExpr;
            _objectiveName = objName;
        }

        /// <summary>
        /// theis method evaluates the rule on the solution object
        /// by compiling the rule expression and returning a simple true or false
        /// depecting wether this rule is satsfied by the solution
        /// </summary>
        /// <param name="sm">the solution manager to work with</param>
        /// <returns>the solutions which satify this rule</returns>
        public override IEnumerable<ISolution> select(ISolutionManger sm)
        {
            //get where condition by compiling it
            var func = ruleExpr.Compile();
            //apply the compiled where condition
            return sm.population.Where(x => x.objectiveEvaluation
                .Any(y=>y.objectiveName ==_objectiveName && func(y)));
        }
    }
}
