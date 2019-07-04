using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace papermilldeploy.Controllers
{
    /// <summary>
    /// Controller handeling the view
    /// </summary>
    public class PapermillController : Controller
    {
        /// <summary>
        /// Returns the View
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}