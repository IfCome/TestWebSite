using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrowdFundingShop.UI.Controllers.WAP
{
    public class DrawnPrizeController : Controller
    {
        //
        // GET: /DrawnPrize/

        public ActionResult Index(long huodongid=0)
        {
            List<Model.OrderInfo> outModel = BLL.OrderInfoBll.GetDrawnPrizeUser(huodongid);
            return View(outModel);
        }
    }
}
