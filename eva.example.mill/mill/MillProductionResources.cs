using System;
using eva.core.framework.framework;

namespace eva.example.mill.mill
{
    /// <summary>
    /// Call representing mill resources
    /// </summary>
    public class MillProductionResources: Resource
    {
        /// <summary>
        /// the mill config
        /// </summary>
        public MillConfiguration milConfig = new MillConfiguration();
        /// <summary>
        /// the orders
        /// </summary>
        public OrderBook orderBook;

        /// <summary>
        /// Constructor to populate mill resources
        /// </summary>
        /// <param name="path">the file path</param>
        public MillProductionResources(String path)
        {
            orderBook = new OrderBook(path);
        }
    }
    /// <summary>
    /// Class representing the mill configuration
    /// </summary>
    public class MillConfiguration
    {
        /// <summary>
        /// The number of machines
        /// </summary>
        public int machines = 2;
        /// <summary>
        /// The setup time unit
        /// </summary>
        public int setup_time = 1;
    }
}
