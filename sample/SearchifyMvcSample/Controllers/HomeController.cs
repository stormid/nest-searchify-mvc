using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nest;

namespace SearchifyMvcSample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(SampleParameters parameters)
        {
            var model = new SampleSearchResults(parameters, new SearchResponse<SampleDocument>());
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}