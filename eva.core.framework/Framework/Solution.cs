using System.Collections.Generic;
using System.Runtime.Serialization;

namespace eva.core.framework.framework
{
    /// <summary>
    /// an abstract class represneting a solution
    /// </summary>
    public abstract class Solution : ISolution, ISerializable
    {
        /// <summary>
        /// the solution id
        /// </summary>
        public int solutionid { get; set; }
        /// <summary>
        /// the batch id
        /// </summary>
        public int batchid { get; set; }
        /// <summary>
        /// clones the solution
        /// </summary>
        /// <returns>A clone of this solution</returns>
        public abstract ISolution doClone();
        /// <summary>
        /// The list of objective evaluations of this solution
        /// </summary>
        public List<EvaluationResult> objectiveEvaluation { get; set; }

        /// <summary>
        /// This method is used by the serializer
        /// </summary>
        /// <param name="info">The serialization info</param>
        /// <param name="context">the streaming context</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // serialize batch id solutin id and objective id by default for all solutions
            info.AddValue("solutionid",solutionid,typeof(int));
            info.AddValue("batchid",batchid,typeof(int));
            info.AddValue("objectiveEvaluation",objectiveEvaluation,typeof(List<EvaluationResult>));
            SerializeData(info, context);
        }
        /// <summary>
        /// Proivdes more ways in which the solution can be serialized
        /// </summary>
        /// <param name="info">The serialization info</param>
        /// <param name="context">the streaming context</param>
        public abstract void SerializeData(SerializationInfo info, StreamingContext context);

        /// <summary>
        /// this constructor is used by de-serializer by default batch id solution id and objective evaluation
        /// are de-serialized for you
        /// </summary>
        /// <param name="info">The serialization info</param>
        /// <param name="context">The sreaming context</param>
        protected Solution(SerializationInfo info, StreamingContext context)
        {
            // deserialize solution id batch id and objective evaluation by default for each
            // serialized string
            solutionid = (int) info.GetValue("solutionid", typeof(int));
            batchid = (int) info.GetValue("batchid", typeof(int));
            objectiveEvaluation = (List<EvaluationResult>) info.GetValue("objectiveEvaluation", typeof(List<EvaluationResult>));
        }
        
        /// <summary>
        /// Empty Constructor
        /// </summary>
        protected Solution(){}
    }
}
