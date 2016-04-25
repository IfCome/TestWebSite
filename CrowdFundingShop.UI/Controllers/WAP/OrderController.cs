using CrowdFundingShop.Utility;
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

        public ActionResult Pay(string huodongids = "", string storecount = "", string allprice = "", string zhongchoucount = "")
        {
            string msg = "OK";
            if (huodongids != "")
            {
                int total = 0;
                foreach (var item in storecount.Split(','))
                {
                    total += Converter.TryToInt32(item);
                }
                //请求微信支付接口
                bool weixinresult = true;
                if (weixinresult)
                {
                    huodongids = huodongids.Remove(huodongids.Length - 1);
                    storecount = storecount.Remove(storecount.Length - 1);
                    allprice = allprice.Remove(allprice.Length - 1);
                    zhongchoucount = zhongchoucount.Remove(zhongchoucount.Length - 1);
                    var flagCount = 0;
                    foreach (var item in huodongids.Split(','))
                    {
                        int ThisOneStoreCount = Converter.TryToInt32(storecount.Split(',')[flagCount]);
                        for (int i = 0; i < ThisOneStoreCount; i++)
                        {
                            Model.OrderInfo entity = new Model.OrderInfo()
                            {
                                ConsumerID = 1,//微信接口获取;
                                HuodongID = Converter.TryToInt64(item),
                                Number = ShoppingNumber(Converter.TryToInt32(allprice.Split(',')[flagCount]), (Converter.TryToInt32(zhongchoucount.Split(',')[flagCount]) + flagCount + 1))
                            };
                            if (msg == "OK")
                            {
                                if (!BLL.OrderInfoBll.Add(entity))
                                {
                                    msg = "网络连接超时";
                                    return Json(new { Message = msg }, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                        flagCount++;
                    }
                }
            }
            return Json(new { Message = msg }, JsonRequestBehavior.AllowGet);
        }

        #region 方法
        public static int ShoppingNumber(int allprice, int currentstore)
        {
            int number = 0;
            number = 1000000 + currentstore;
            return number;
        }
        #endregion
    }
}
