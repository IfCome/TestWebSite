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
            if (listorder != null)
            {
                foreach (var item in listorder)
                {
                    item.Numbers = BLL.OrderInfoBll.GetNumberByHuoDongAndUser(huodongid, item.ID);
                }
            }
            outModel.LIstOrderInfo = listorder;
            return View(outModel);
        }
    }
}
