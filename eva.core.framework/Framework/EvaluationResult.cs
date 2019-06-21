using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eva.core.framework.framework
{
    /// <summary>
    /// A Class depicting an Evaulation Of an Objective
    /// </summary>
    [Serializable]
    public class EvaluationResult
    {
        /// <summary>
        /// the objective name
        /// </summary>
        public string objectiveName { get; set;}
        /// <summary>
        /// The evaluation of which objective
        /// </summary>
        public double evaluationResult { get; set; }

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="objName">the objective name</param>
        /// <param name="value">the value of objective evaluation</param>
        public EvaluationResult(string objName, double value)
        {
            objectiveName = objName;
            evaluationResult = value;
        }
    }
}
