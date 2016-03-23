using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrowdFundingShop.UI.Controllers.PC
{
    public class HomeController : PCBaseController
    {
        //
        // GET: /Home/

        public ActionResult IndexPage()
        {
            // 热门商品

            // 后台日志
            List<Model.BackgroundUserInfo_log> logList = BLL.BackgroundUserBll_log.GetTop10Logs();
            ViewBag.LogList = logList;

            // 不知道写啥


            return View();
        }

    }
}
