using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrowdFundingShop.UI.Models.PC;
using CrowdFundingShop.Utility;

namespace CrowdFundingShop.UI.Controllers.PC
{
    public class HomeController : PCBaseController
    {
        //
        // GET: /Home/

        public ActionResult IndexPage()
        {
            // 热门商品

            DataTable huodongSimpleInfoTable = BLL.HuoDongInfoBll.GetTop10SimpleInfo();
            List<HomeIndexPageModelOut> huodongSimpleInfoList = new List<HomeIndexPageModelOut>();
            if (huodongSimpleInfoTable != null && huodongSimpleInfoTable.Rows.Count > 0)
            {
                foreach (DataRow row in huodongSimpleInfoTable.Rows)
                {
                    huodongSimpleInfoList.Add(new HomeIndexPageModelOut()
                    {
                        GoodsID = Converter.TryToInt64(row["GoodsID"]),
                        GoodsName = Converter.TryToString(row["GoodsName"]),
                        LastestCustomer = Converter.TryToString(row["LastestCustomer"]),
                        ShareCount = Converter.TryToInt32(row["ShareCount"]),
                        OrderCount = Converter.TryToInt32(row["OrderCount"]),
                        Progress = (float)Converter.TryToDouble(row["Progress"]),
                        DailyIncrease = Converter.TryToInt32(row["DailyIncrease"]),
                        Describe = Converter.TryToString(row["Describe"]),
                        HuodongNumber = Converter.TryToInt32(row["HuodongNumber"])
                    });
                }
            }
            ViewBag.HuoDongList = huodongSimpleInfoList;

            // 后台日志
            List<Model.BackgroundUserInfo_log> logList = BLL.BackgroundUserBll_log.GetTop10Logs();
            ViewBag.LogList = logList;

            // 不知道写啥


            return View();
        }

    }
}
