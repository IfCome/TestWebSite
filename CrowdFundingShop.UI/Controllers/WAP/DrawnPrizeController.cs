using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrowdFundingShop.UI.Controllers.WAP
{
    public class DrawnPrizeController : OauthController
    {
        //
        // GET: /DrawnPrize/

        public ActionResult Index(long huodongid = 0)
        {
            List<Model.OrderInfo> listorder = BLL.OrderInfoBll.GetDrawnPrizeUser(huodongid);
            Model.HuoDongInfo outModel = BLL.HuoDongInfoBll.GetLuckNumberByID(huodongid);
            outModel.LIstOrderInfo = listorder;
            return View(outModel);
        }
    }
}
