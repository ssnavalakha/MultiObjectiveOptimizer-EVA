using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eva.core.framework.framework
{
    //TODO : should inherit from an interface
    /// <summary>
    /// An Abstract Calss representing an objective
    /// </summary>
    public abstract class Objective
    {
        /// <summary>
        /// name of the objective
        /// </summary>
        public String name;
        /// <summary>
        /// The target of the objective
        /// </summary>
        public double target;
        /// <summary>
        /// Precision of the objective
        /// </summary>
        public double precision;
        /// <summary>
        /// Depitcts if the objective is active
        /// </summary>
        public bool active;
        /// <summary>
        /// Specifies wether reducing this objective is advantageous or maximizing this objective is
        /// </summary>
        public bool isAsc;
        
        /// <summary>
        /// A method to run the evaluation
        /// </summary>
        /// <param name="solution">the solution against which this object is to evaluated</param>
        /// <returns>the evaluated result of the solution</returns>
        public abstract double evaluate(ISolution solution);

        /// <summary>
        /// Constructor taking parameters from config file
        /// </summary>
        /// <param name="para">the parameters read from a config file</param>
        protected Objective(Dictionary<String, Object> para)
        {
            active = true;
            precision = 1;
            foreach (var parameter in para)
            {
                if (parameter.Key == "name")
                    name = (String)parameter.Value;
                if (parameter.Key == "target")
                    target = Convert.ToDouble(parameter.Value);
                if (parameter.Key == "precision")
                    precision = Convert.ToDouble(parameter.Value);
                if (parameter.Key == "active")
                    active = Convert.ToBoolean(parameter.Value);
            }
        }

        /// <summary>
        /// Empty COnstructor
        /// </summary>
        protected Objective()
        {
        }
    }
}
