using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrowdFundingShop.UI.Controllers.WAP
{
    public class UserCenterController : Controller
    {
        //
        // GET: /UserInfo/

        public ActionResult Index()
        {
            long consumerid = 1;//要走微信端
            //查3个关键数据
            Model.ConsumerInfo consumerinfo = BLL.OrderInfoBll.GetKeyCount(consumerid);
            return View(consumerinfo);
        }

        public ActionResult PurchaseHistory(int type = 0)
        {
            long consumerid = 1;//微信验证获取得到
            List<Model.GoodsBaseInfo> outModel = BLL.OrderInfoBll.GetList(type, consumerid);
            ViewBag.Type = type;
            return View(outModel);
        }
    }
}
