using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace eva.core.framework.framework
{
    /// <summary>
    /// represents a solution
    /// </summary>
    public interface ISolution
    {
        /// <summary>
        /// the id of the solutions 
        /// </summary>
        int solutionid { get; set; }
        /// <summary>
        /// the batch id of the solution 
        /// </summary>
        int batchid { get; set; }
        /// <summary>
        /// A method to generate clones of this solution
        /// </summary>
        /// <returns>A clone of this solution</returns>
        ISolution doClone();
        /// <summary>
        /// The list of objective evaluations of this solution
        /// </summary>
        List<EvaluationResult> objectiveEvaluation { get; set; }
    }
}
