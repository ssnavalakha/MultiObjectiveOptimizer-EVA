using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace papermilldeploy.Controllers
{
    public class PapermillController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}