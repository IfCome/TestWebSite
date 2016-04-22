using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CrowdFundingShop.UI.Controllers.WAP
{
    public class GoodsListController : Controller
    {
        //
        // GET: /GoodsList/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }
    }
}
