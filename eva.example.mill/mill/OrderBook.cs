using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eva.core.framework.framework;

namespace eva.example.mill.mill
{
    /// <summary>
    /// Represents a list of orders
    /// </summary>
    public class OrderBook
    {
        /// <summary>
        /// the order list in this book
        /// </summary>
        public List<Order> orders;
        /// <summary>
        /// Constructor to create order book from a file
        /// </summary>
        /// <param name="path">the file path</param>
        public OrderBook(string path)
        {
            orders = new List<Order>();
            var textfileReader = new TextFileReader<Order>(path);

            orders = textfileReader.ReadFileAndConvert((row, header) =>
            {
                var orderId = Convert.ToInt32(row.ElementAt(Array.IndexOf(header,"orderid")));
                var productcode = Convert.ToInt32(row.ElementAt(Array.IndexOf(header,"productcode")));
                var duedate = Convert.ToInt32(row.ElementAt(Array.IndexOf(header,"duedate")));
                var highpriority = Convert.ToBoolean(row.ElementAt(Array.IndexOf(header,"highpriority")));
                var productioncycles = Convert.ToInt32(row.ElementAt(Array.IndexOf(header,"productioncycles")));
                return new Order(orderId,productcode,duedate,highpriority,productioncycles);

            }).ToList();
        }
    }

    /// <summary>
    /// Represents Orders
    /// </summary>
    [Serializable]
    public class Order
    {
        /// <summary>
        /// The order id
        /// </summary>
        public int orderId { get; }
        /// <summary>
        /// the product code
        /// </summary>
        public int productCode { get; }
        /// <summary>
        /// the due date
        /// </summary>
        public int dueDate { get; }
        /// <summary>
        /// is the order high priority?
        /// </summary>
        public bool highPriority { get; }
        /// <summary>
        /// number of production cycles
        /// </summary>
        public int productionCycles { get; }
        /// <summary>
        /// Constructor to create orders
        /// </summary>
        /// <param name="orderid">The order id</param>
        /// <param name="productcode">the product code</param>
        /// <param name="duedate"> the due date</param>
        /// <param name="highpriority"> is the order high priority?</param>
        /// <param name="productioncycles">number of production cycles</param>
        public Order(int orderid,int productcode,int duedate,bool highpriority,int productioncycles)
        {
            orderId = orderid;
            productCode = productcode;
            dueDate = duedate;
            highPriority = highpriority;
            productionCycles = productioncycles;
        }
    }

    
}
