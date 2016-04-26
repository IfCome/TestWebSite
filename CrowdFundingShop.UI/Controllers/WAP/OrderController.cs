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
            long consumerid = 1;//微信接口获取;
            string msg = "OK";
            try
            {
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
                                    ConsumerID = consumerid,
                                    HuodongID = Converter.TryToInt64(item),
                                    Number = ShoppingNumber(Converter.TryToInt64(item), consumerid)
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
                            //买完之后删除购物车该商品
                            Model.ShoppingCart scart = new Model.ShoppingCart()
                            {
                                ConsumerID = 1,
                                HuoDongID = Converter.TryToInt32(item)
                            };
                            BLL.ShoppingCartBll.DeleteByHuoDongID(scart);
                            flagCount++;
                        }
                    }
                }
            }
            catch
            {
                return Json(new { Message = "网络连接超时" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Message = msg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MyLuckInfo()
        {
            long consumerid = 1;//微信验证获取得到
            List<Model.GoodsBaseInfo> outModel = BLL.OrderInfoBll.GetList(2, consumerid, 1);
            return View();
        }

        #region 方法
        public static int ShoppingNumber(long huodongid, long consumerid)
        {
            int number = 0;
            //查在这个人的这个活动ID下现在最大的号码是多少
            int maxnumber = BLL.OrderInfoBll.GetMaxNumber(huodongid, consumerid);
            number = maxnumber + 1;
            return number;
        }
        #endregion
    }
}
