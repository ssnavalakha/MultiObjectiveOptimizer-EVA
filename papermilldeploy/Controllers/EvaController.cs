using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using eva.core.framework.framework;
using eva.example.mill.mill;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using papermilldeploy.WebCore;

namespace papermilldeploy.Controllers
{
    /// <summary>
    /// Controller for handleing requests
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EvaController : ControllerBase
    {
        /// <summary>
        /// The config file containing stuff
        /// </summary>
        private IConfiguration _config;
        private readonly IHostingEnvironment environment;

        public EvaController(IConfiguration config, IHostingEnvironment env)
        {
            _config = config;
            environment = env;
        }

        /// <summary>
        /// takes a form input with a file reads it and runs the optimizer
        /// </summary>
        /// <returns>the solutions to be displayed</returns>
        [HttpPost]
        [Route("runeva")]
        public String runeva()
        {
            string filepath = null;
            // save the file on the server
            if (HttpContext.Request.Form.Files.Any())
            {
                var httpPostedFile = HttpContext.Request.Form.Files[0];

                if (httpPostedFile != null)
                {
                    filepath = Path.Combine(environment.WebRootPath+"/UploadedFiles", httpPostedFile.FileName);
                    using (var fileStream = new FileStream(filepath, FileMode.Create))
                    {
                        httpPostedFile.CopyTo(fileStream);
                    }
                }
            }

            var sm = new ProductionSolutionManager(new MySqlConnectionRetriever(_config));
            SolutionManager.SetResource(new MillProductionResources(filepath));
            sm.run();
            var collectionWrapper = new
            {
                slns = sm.population.Select(x => new { x.solutionid, x.batchid, x.objectiveEvaluation })
            };
            return SerializeAndReturn(collectionWrapper);
        }

        /// <summary>
        /// Gets details of particular solution
        /// </summary>
        /// <param name="solutionId">the solution id</param>
        /// <param name="batchId">the batch id</param>
        /// <returns>the solution</returns>
        [HttpGet]
        [Route("fetchSolutionDetails")]
        public String fetchSolutionDetails(int solutionId, int batchId)
        {
            var dbapi = new DbApi(new MySqlConnectionRetriever(_config));
            string fetchdValue = dbapi.FetchSerializedDataFromDb(solutionId, batchId);

            var deserializeObject = JsonConvert.DeserializeObject<ProductionSchedule>(fetchdValue);
            var propsedSchedule = deserializeObject.getProposedOrder();
            List<Object> objList = new List<object>();
            string[] details = new string[] { "machineId", "orderId", "dueDate", "highPriority", "productCode", "productionCycles" };
            for (int i = 0; i < propsedSchedule.Count; i++)
            {
                var values = new object[propsedSchedule[i].Count];
                for (int j = 0; j < propsedSchedule[i].Count; j++)
                {
                    var orders = propsedSchedule[i][j];
                    values[j] =
                        new
                        {
                            machineId = i,
                            orders.orderId,
                            orders.dueDate,
                            orders.highPriority,
                            orders.productCode,
                            orders.productionCycles
                        };
                }
                var collectionWrapper = new
                {
                    values,
                    details
                };
                objList.Add(collectionWrapper);
            }
            return SerializeAndReturn(objList);
        }

        /// <summary>
        /// fetches the objectives of a solution manager
        /// </summary>
        /// <returns>the objectives</returns>
        [HttpGet]
        [Route("getObjectives")]
        public String getObjectives()
        {
            var sm = new ProductionSolutionManager(new MySqlConnectionRetriever(_config));
            var collectioWrapper = new
            {
                objs = sm.objs.Select(x => x.name)
            };
            return SerializeAndReturn(collectioWrapper);
        }

        /// <summary>
        /// A class representing the structure of a rule
        /// </summary>
        public class RuleElements
        {
            /// <summary>
            /// The left hand side of the rule
            /// </summary>
            public String lhs { get;set;}
            /// <summary>
            /// The right hand side of the rule
            /// </summary>
            public String rhs { get;set;}
            /// <summary>
            /// The equality sign of the rule
            /// </summary>
            public String equality { get;set;}
        }

        /// <summary>
        /// takes rules and batch id as input form
        /// </summary>
        /// <returns>the solution after running them against the rules</returns>
        [HttpPost]
        [Route("runWithRules")]
        public String runWithRules()
        {
            var ruleDefinations = new List<RuleElements>();
            //.FirstOrDefault(x=>x == "ruleDef")
            //foreach (var objs in HttpContext.Request.Form["ruleDef"])
            //{
            //    ruleDefinations.Add(DeserializeData<RuleElements>(objs));
            //}
            ruleDefinations= DeserializeData<List<RuleElements>>(HttpContext.Request.Form["ruleDef"]);
            var sm = new ProductionSolutionManager(new MySqlConnectionRetriever(_config));
            var bid = Convert.ToInt32(HttpContext.Request.Form["batchId"][0]);
            sm.rerun(convertToRuleList(ruleDefinations), bid);
            var collectionWrapper = new
            {
                slns = sm.population.Select(x => new { x.solutionid, x.batchid, x.objectiveEvaluation })
            };
            return SerializeAndReturn(collectionWrapper);
        }


        /// <summary>
        /// do it in an elegant way please
        /// </summary>
        /// <param name="ruleDefinations"></param>
        /// <returns></returns>
        private List<Rule> convertToRuleList(List<RuleElements> ruleDefinations)
        {
            var returnMe = new List<Rule>();
            ruleDefinations.ForEach(x =>
            {
                var lhs = x.lhs;
                var rhs = x.rhs;
                var eq = x.equality;

                switch (eq.ToString())
                {
                    case ">=":
                        var rule = new Rule(ExpressionType.GreaterThanOrEqual, rhs.ToString(), lhs.ToString());
                        returnMe.Add(rule);
                        break;
                    case "<=":
                        var rule1 = new Rule(ExpressionType.LessThanOrEqual, rhs.ToString(), lhs.ToString());
                        returnMe.Add(rule1);
                        break;

                }
            });
            return returnMe;
        }

        /// <summary>
        /// Serializes the object using JavaScriptSerializer
        /// </summary>
        /// <param name="objectToWrap">the object to serialize</param>
        /// <returns>the serialized object</returns>
        private String SerializeAndReturn(Object objectToWrap)
        {
            return JsonConvert.SerializeObject(objectToWrap);
        }

        /// <summary>
        /// Deserializes a string using JavaScriptSerializer
        /// </summary>
        /// <typeparam name="T">the object to be returned</typeparam>
        /// <param name="data">the data to deserialize</param>
        /// <returns>the deserialized object</returns>
        private T DeserializeData<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}