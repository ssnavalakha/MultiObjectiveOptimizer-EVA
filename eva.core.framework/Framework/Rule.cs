using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace eva.core.framework.framework
{
    /// <summary>
    /// a class representing a rule
    /// </summary>
    public class Rule
    {
        /// <summary>
        /// the property name which stores the objective value
        /// </summary>
        public string ComparisonPredicate { get; set; }
        /// <summary>
        /// an expression which consists of the equality sign used
        /// </summary>
        public ExpressionType ComparisonOperator { get; set; }
        /// <summary>
        /// the value to be compared
        /// </summary>
        public string ComparisonValue { get; set; }
        /// <summary>
        /// the objectiveName against which the rule is defined
        /// </summary>
        public string objectiveName { get; set; }
        
        /// <summary>
        /// rule constructor
        /// </summary>
        /// <param name="comparisonOperator">The Operator expression</param>
        /// <param name="comparisonValue">the value against which this comparrision is to be done</param>
        /// <param name="objName">the objective name</param>
        public Rule(ExpressionType comparisonOperator, string comparisonValue,string objName)
        {
            ComparisonPredicate = "evaluationResult";
            ComparisonOperator = comparisonOperator;
            ComparisonValue = comparisonValue;
            objectiveName = objName;
        }
    }
}
