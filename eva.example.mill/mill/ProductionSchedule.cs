using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using eva.core.framework.framework;
using Newtonsoft.Json.Linq;

namespace eva.example.mill.mill
{
    /// <summary>
    /// Represents a mill solution
    /// </summary>
    [Serializable]
    public class ProductionSchedule : Solution
    {
        /// <summary>
        /// This constructir is used by the deserilizer
        /// </summary>
        /// <param name="info">the serialization info</param>
        /// <param name="context">the serialization context</param>
        public ProductionSchedule(SerializationInfo info, StreamingContext context)
        :base(info,context)
        {
            var schedule=new List<List<Order>>();
            var orderObjects = (Object[][])info.GetValue("proposedSchedule", typeof(Object[][]));
            foreach (var machines in orderObjects)
            {
                var orderSchedule=new List<Order>();
                foreach (var order in machines)
                {
                    var jOrderObject = order as JObject;
                    var orderId = Convert.ToInt32(jOrderObject.GetValue("orderId"));
                    var productCode = Convert.ToInt32(jOrderObject.GetValue("productCode"));
                    var dueDate = Convert.ToInt32(jOrderObject.GetValue("dueDate"));
                    var highPriority = Convert.ToBoolean(jOrderObject.GetValue("highPriority"));
                    var productionCycles = Convert.ToInt32(jOrderObject.GetValue("productionCycles"));
                    var parameters = new List<String>();
                    parameters.Add(orderId.ToString());
                    parameters.Add(productCode.ToString());
                    parameters.Add(dueDate.ToString());
                    parameters.Add(highPriority.ToString());
                    parameters.Add(productionCycles.ToString());
                    orderSchedule.Add(new Order(orderId,productCode,dueDate,highPriority,productionCycles));
                }
                schedule.Add(orderSchedule);
            }
            proposedSchedule = schedule;
        }
        /// <summary>
        /// the proposed solution
        /// </summary>
        private List<List<Order>> proposedSchedule;
        /// <summary>
        /// The initial mill solution setup
        /// </summary>
        public ProductionSchedule()
        {
            var millResources = SolutionManager.GetResource();
            proposedSchedule = new List<List<Order>>();
            for (int i = 0; i < ((MillProductionResources) millResources).milConfig.machines; i++)
                proposedSchedule.Add(new List<Order>());
            objectiveEvaluation =new List<EvaluationResult>();
        }
        
        /// <summary>
        /// Returns the mill resources
        /// </summary>
        /// <returns>returns the static mill resources</returns>
        public Resource getResources()
        {
            return SolutionManager.GetResource();
        }

        /// <summary>
        /// Gets the current proposed order schedule
        /// </summary>
        /// <returns>the proposed order this solution carries</returns>
        public List<List<Order>> getProposedOrder()
        {
            var returnMe=new List<List<Order>>();
            proposedSchedule.ForEach(ordering =>
            {
                var orderList=new List<Order>();
                ordering.ForEach(order =>
                {
                    orderList.Add(order);
                });
                returnMe.Add(orderList);
            });
            return returnMe;
        }

        /// <summary>
        /// Setter for the proposed order
        /// </summary>
        /// <param name="newProposedList">the new order list</param>
        public  void setProposedList(List<List<Order>> newProposedList)
        {
            proposedSchedule= newProposedList;
        }

        /// <summary>
        /// Creates a clone of this solution
        /// </summary>
        /// <returns>the cloned solution</returns>
        public override ISolution doClone()
        {
            var result = new ProductionSchedule();
            result.proposedSchedule = new List<List<Order>>();
            proposedSchedule.ForEach(ordering =>
            {
                var orderList=new List<Order>();
                ordering.ForEach(order =>
                {
                    orderList.Add(order);
                });
                result.proposedSchedule.Add(orderList);
            });
            result.objectiveEvaluation = new List<EvaluationResult>();
            return result;
        }
        
        /// <summary>
        /// Adds more data during serialization 
        /// </summary>
        /// <param name="info">The serialization info</param>
        /// <param name="context">the streaming context</param>
        public override void SerializeData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("proposedSchedule",proposedSchedule,typeof(List<List<Order>>));
        }
        
        /// <summary>
        /// Checks if an object is equal to this object
        /// </summary>
        /// <param name="obj">the object to be tested</param>
        /// <returns>the result of the test</returns>
        public override bool Equals(object obj)
        {
            var temp = obj as ProductionSchedule;
            if (temp == null)
                return false;
            else
            {
                var result = true;
                for (int i=0;i<getProposedOrder().Count;i++)
                {
                    result = result & checkOrderEquality(getProposedOrder()[i], temp.getProposedOrder()[i]);
                    if (!result)
                        return false;
                }
                return result;
            }
        }
        /// <summary>
        /// Checks if 2 order lists are equal
        /// </summary>
        /// <param name="o1">the first order list</param>
        /// <param name="o2">the seond order list</param>
        /// <returns>the result of the test</returns>
        private bool checkOrderEquality(IEnumerable<Order> o1,IEnumerable<Order> o2)
        {
            if (o1.Count() != o2.Count())
                return false;
            else
            {
                for (int i = 0; i < o1.Count(); i++)
                {
                    if (o1.ElementAt(i).orderId != o2.ElementAt(i).orderId)
                        return false;
                }
                return true;
            }
        }
        
        /// <summary>
        /// since equals only depends on proposed schedule so should hashcode 
        /// </summary>
        /// <returns>hashcode</returns>
        //TODO: move hashcode generation into list creation logic for better performance
        public override int GetHashCode()
        {
            int hash=0;
            foreach (var orderList in proposedSchedule)
            {
                foreach (var order in orderList)
                {
                    hash = hash + order.orderId;   
                }
            }
            return hash;
        }
    }
}
