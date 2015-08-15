using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace TaskMgtSystem.Controllers.Test
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult Index()
        {
            return View("Test");
        }

        public ActionResult GetDate()
        {
            var now = DateTime.Now;

            var obj = new TestDate();
            obj.LocalDate = now.ToLocalTime();
            obj.LocalStr = now.ToLocalTime().ToString();
            obj.UTCDate = now.ToUniversalTime();
            obj.UTCStr = now.ToUniversalTime().ToString();

            return Json(obj);
        }

        public ActionResult PostDate([FromBody]TestDate testDate)
        {
            return Json(testDate);
        }

        public class TestDate {
            public DateTime LocalDate { get; set; }
            public string LocalStr { get; set; }
            public DateTime UTCDate { get; set; }
            public string UTCStr { get; set; }
        }

    }
}
