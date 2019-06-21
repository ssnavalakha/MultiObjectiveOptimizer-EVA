using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace eva.core.framework.framework
{
    /// <summary>
    /// Converts rules to expressions which can then be converted to executble code
    /// </summary>
    public sealed class RulesExpressionGenerator
    {
        /// <summary>
        ///  a method used to precompile rules for a provided type
        /// </summary>
        /// <typeparam name="T">The type on which this rule is to be executed eg ISolution</typeparam>
        /// <param name="rules">the rules defined by the user</param>
        /// <returns>an expression dictionay which on compiling reveals the code to on how t evaluate a particular rule</returns>
        public static Dictionary<string,Expression<Func<T, bool>>> CreateRuleExpression<T>(List<Rule> rules)
        {
            var compiledRules = new Dictionary<string,Expression<Func<T, bool>>>();

            // loop through the rules and compile them against the properties of the supplied shallow object 
            rules.ForEach(rule =>
            {
                //create expression tree of type parameter<T>
                var genericType = Expression.Parameter(typeof(T));
                //create property accessor

                // do this to make more generic rules
                var lhs = MemberExpression.Property(genericType, rule.ComparisonPredicate);
                //get property type of value
                var propertyType = typeof(T).GetProperty(rule.ComparisonPredicate).PropertyType;
                //create value expression
                var rhs = Expression.Constant(Convert.ChangeType(rule.ComparisonValue, propertyType));
                // create boolean condition Condition LHS RHS
                var binaryExpression = Expression.MakeBinary(rule.ComparisonOperator, lhs, rhs);
                // create delegate type
                compiledRules.Add(rule.objectiveName,Expression.Lambda<Func<T,bool>>(binaryExpression, genericType));
            });

            // return the formulated expression to the caller
            return compiledRules;
        }
    }
}
