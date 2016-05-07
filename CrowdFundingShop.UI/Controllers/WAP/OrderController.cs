using CrowdFundingShop.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrowdFundingShop.UI.Controllers.WAP
{
    public class OrderController : OauthController
    {
        //
        // GET: /Order/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Pay(string huodongids = "", string storecount = "", string allprice = "", string zhongchoucount = "")
        {
            try
            {
                long consumerid = Identity.LoginConsumer.ID;//微信接口获取;
                bool reslut = false;//用来标记交易阶段的状态
                var flagCount = 0;//标记位置
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
                                        reslut = BLL.OrderInfoBll.Add(entity);
                                        if (!reslut)
                                        {
                                            msg = "网络连接超时";
                                            return Json(new { Message = msg }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                }
                                //买完之后删除购物车该商品
                                Model.ShoppingCart scart = new Model.ShoppingCart()
                                {
                                    ConsumerID = Identity.LoginConsumer.ID,
                                    HuoDongID = Converter.TryToInt32(item)
                                };
                                BLL.ShoppingCartBll.DeleteByHuoDongID(scart);
                                flagCount++;
                            }
                        }
                        //判断是不是该商品的最后最后一个购买者，确定开不开奖
                        if (reslut)
                        {
                            flagCount = 0;
                            huodongids = huodongids.Remove(huodongids.Length - 1);
                            foreach (var item in huodongids.Split(','))
                            {
                                int maxnum = BLL.OrderInfoBll.GetMaxNumber(Converter.TryToInt32(item), consumerid) - 1000000;
                                if (Converter.TryToInt32(allprice.Split(',')[flagCount]) == maxnum)
                                {
                                    KaiJiang(Converter.TryToInt32(allprice), Converter.TryToInt64(item));
                                }
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
            catch (Exception e)
            {
                BLL.BackgroundUserBll_log.AddLog("标记120", "支付时发生：" + e.Message, "0.0.0.0");
                return null;
            }
        }

        public ActionResult MyLuckInfo()
        {
            long consumerid = Identity.LoginConsumer.ID;//微信验证获取得到
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
        public static bool KaiJiang(int allPrice, long huodongid)
        {
            bool result = false;
            int lucknumber = 0;
            int sum = 0;
            string randomS = string.Empty;
            int x = 0;
            for (int i = 0; i < 20; i++)
            {
                Random random = new Random();
                x = random.Next(1000000, 1000000 + allPrice + 1);
                sum += x;
                randomS += x + ",";
            }
            //记日志（存活动ID和random以便查询）
            BLL.OrderInfoBll.AddRandom(randomS, huodongid);
            //产生幸运号码后查出获奖者并更新活动表
            lucknumber = sum % (allPrice + 1);
            Model.OrderInfo orderinfo = BLL.ConsumerInfoBll.GetByNumber(lucknumber, huodongid);
            Model.HuoDongInfo huodonginfo = new Model.HuoDongInfo()
            {
                ID = huodongid,
                State = 30,
                LuckDogID = orderinfo.ConsumerID,
                LuckNumber = lucknumber
            };
            result = BLL.HuoDongInfoBll.Update(huodonginfo);
            return result;
        }
        #endregion
    }
}
