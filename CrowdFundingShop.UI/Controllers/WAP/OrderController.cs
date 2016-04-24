using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrowdFundingShop.UI.Controllers.WAP
{
    public class OrderController : Controller
    {
        //
        // GET: /Order/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Pay(int price)
        {
            ViewBag.Price = price;
            return View();
        }

    }
}
