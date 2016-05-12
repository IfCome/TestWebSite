using CrowdFundingShop.Model;
using CrowdFundingShop.Utility;
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
    public class GoodsListController : WAPBaseController
    {
        //
        // GET: /GoodsList/
        public ActionResult Index()
        {
            return View();
        }
        //商品列表页
        public ActionResult List(string userinfo)
        {
            ViewBag.UserInfo = userinfo;
            return View();
        }
        //获取商品列表
        public ActionResult GetGoodsList(int pageSize, int currentPage, int category = 0, int ishot = 0, int jiexiaotype = 0, string keyWords = "", string huoDongState = "")
        {
            List<Model.GoodsBaseInfo> goodsInfoList = new List<Model.GoodsBaseInfo>();
            int allCount = 0;
            goodsInfoList = BLL.GoodsBaseInfoBll.GetList(pageSize, currentPage, keyWords, category, (huoDongState == "-1" ? "" : huoDongState), ishot, jiexiaotype, out allCount);
            if (goodsInfoList != null)
            {
                return Json(new
                {
                    Rows = goodsInfoList.Select(g => new
                    {
                        g.ID,
                        g.GoodsName,
                        g.DetailIcons,
                        Describe = (g.Describe.Length > 50 ? (g.Describe.Remove(50, g.Describe.Length - 50) + "......") : g.Describe),
                        g.Price,
                        g.ShowIcons,
                        g.Category,
                        g.CreateTime,
                        g.State,
                        g.ZhongChouCount,
                        ZhongChouPercent = (g.Price != "0" && g.Price != "" && g.ZhongChouCount != 0) ? ((g.ZhongChouCount * 100.0 / Converter.TryToInt32(g.Price)) < 1 ? 2 : (g.ZhongChouCount * 100.0 / Converter.TryToInt32(g.Price))) : 0
                    }),
                    AllCount = allCount
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Rows = "", AllCount = 0 }, JsonRequestBehavior.AllowGet);
        }
        //商品种类
        public ActionResult GetCategoryInfo(int parentID)
        {
            List<Model.CategoryInfo> categoryInfoList = new List<Model.CategoryInfo>();
            categoryInfoList = BLL.CategoryInfoBll.GetListByParentID(parentID, "WAP");
            if (categoryInfoList != null)
            {
                return Json(new
                {
                    Rows = categoryInfoList.Select(g => new
                    {
                        ID = g.ID,
                        ParentID = g.ParentId,
                        CategoryName = g.CategoryName
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        //商品详情
        public ActionResult Detail(string goodsID = "0")
        {
            Model.GoodsBaseInfo outModel = BLL.GoodsBaseInfoBll.GetGoodsInfoByID(goodsID);
            outModel.ZhongChouPercent = (outModel.Price != "0" && outModel.Price != "" && outModel.ZhongChouCount != 0) ? ((outModel.ZhongChouCount * 100.0 / Converter.TryToInt32(outModel.Price)) < 1 ? 2 : (outModel.ZhongChouCount * 100.0 / Converter.TryToInt32(outModel.Price))) : 0;
            return View(outModel);
        }
        //添加至我的购物车
        public ActionResult AddToCart(long HuoDongID = 0, int StoreCount = 0, int isCallback = 0)
        {
            string errorType = "";
            string msg = "OK";
            bool result = false;
            Model.ShoppingCart shoppingCart = new Model.ShoppingCart()
            {
                ConsumerID = Identity.LoginConsumer.ID,//通过微信账号来判断该角色的顾客ID
                HuoDongID = HuoDongID,
                StoreCount = StoreCount,
                Type = 1
            };
            #region 先查是否存在，是则累加，否则新增记录
            bool isExist = BLL.ShoppingCartBll.IsExistShoppingCartByHuoDongID(HuoDongID, Identity.LoginConsumer.ID);
            if (isExist)
            {
                result = BLL.ShoppingCartBll.UpdateStoreCount(shoppingCart);
            }
            else
            {
                result = BLL.ShoppingCartBll.Add(shoppingCart);
            }
            #endregion
            if (!result)
            {
                errorType = "alert";
                msg = "添加失败，请重试";
            }
            return Json(new { Message = msg, ErrorType = errorType }, JsonRequestBehavior.AllowGet);
        }
        //我的购物车
        public ActionResult MyShoppingCart()
        {
            long consumerID = Identity.LoginConsumer.ID;//通过微信账号来判断该角色的顾客ID
            List<Model.ShoppingCart> shoppingCartList = BLL.ShoppingCartBll.GetShoppingInfoByID(consumerID);
            if (shoppingCartList != null)
            {
                foreach (var item in shoppingCartList)
                {
                    item.ZhongChouPercent = (item.Price != "0" && item.Price != "" && item.ZhongChouCount != 0) ? ((item.ZhongChouCount * 100.0 / Converter.TryToInt32(item.Price)) < 1 ? 2 : (item.ZhongChouCount * 100.0 / Converter.TryToInt32(item.Price))) : 0;
                }
            }
            return View(shoppingCartList);
        }
        //删除购物车内物品
        public ActionResult Delete(int shoppingid = 0)
        {
            string errorType = "";
            string msg = "OK";
            Model.ShoppingCart shoppingCart = new Model.ShoppingCart()
            {
                ID = shoppingid,
                ConsumerID = Identity.LoginConsumer.ID,//通过微信账号来判断该角色的顾客ID
            };
            bool result = BLL.ShoppingCartBll.Delete(shoppingCart);
            if (!result)
            {
                errorType = "alert";
                msg = "删除失败，请重试";
            }
            return Json(new { Message = msg, ErrorType = errorType }, JsonRequestBehavior.AllowGet);
        }
        //结算进入购买页
        public ActionResult BuyIt(string shoppingcartid = "")
        {
            try
            {
                List<Model.ShoppingCart> outModel = null;
                if (shoppingcartid != "")
                {
                    //1、更新购物车数量shopping
                    string[] strs = shoppingcartid.Remove(shoppingcartid.Length - 1).Split(',');
                    foreach (var item in strs)
                    {
                        Model.ShoppingCart entity = new Model.ShoppingCart()
                        {
                            ID = Converter.TryToInt64(item.Split(':')[0]),
                            StoreCount = Converter.TryToInt32(item.Split(':')[1]),
                            ConsumerID = Identity.LoginConsumer.ID,//通过微信账号来判断该角色的顾客ID
                            Type = 2
                        };
                        if (!BLL.ShoppingCartBll.UpdateStoreCount(entity))
                        {
                            Response.Write("<script>Model.message('网络不稳定，请稍后再试');<script>");
                            return null;
                        }
                    }
                    //2、查当前购物车的信息返回前台
                    string sids = string.Empty;
                    foreach (var item in strs)
                    {
                        sids += item.Split(':')[0] + ",";
                    }
                    sids = sids.Remove(sids.Length - 1);
                    outModel = BLL.ShoppingCartBll.GetShoppingInfoBySID(sids);

                    #region 向微信发起请求
                    string openId = Identity.LoginConsumer.WeiXinAccount;//Request.QueryString["openid"].ToString();
                    try
                    {
                        //string body = string.Empty;
                        //int total_fee = 0;
                        //foreach (var item in outModel)
                        //{
                        //    body += item.GoodsName + "X" + item.StoreCount + "；";
                        //    total_fee += Converter.TryToInt32(item.Price);
                        //}
                        string body = "测试机";
                        int total_fee = 1;
                        string orderno = DateTime.Now.ToString("yyyyMMddhhmmss");

                        #region 向微信下订单
                        WxIncomeHelp client = new WxIncomeHelp();
                        UnifyOrder entity = new UnifyOrder();
                        entity.appid = ConfigurationManager.AppSettings["AppID"].ToString();
                        entity.mch_id = ConfigurationManager.AppSettings["mch_id"].ToString();
                        //随机字符串不长于32位
                        entity.nonce_str = JsIncomHelp.GetRandCode(32);
                        //商户订单号32个字符内、可包含字母
                        entity.out_trade_no = orderno;
                        entity.body = body;
                        entity.total_fee = total_fee;//
                        entity.spbill_create_ip = Request.UserHostAddress;
                        string url = ConfigurationManager.AppSettings["commonPayForReturnUrl"];
                        entity.notify_url = url;
                        entity.trade_type = "JSAPI";
                        entity.openid = openId;
                        string key = ConfigurationManager.AppSettings["key"].ToString();
                        string xmlStr = client.DoDataForIncomeWeiXin(entity, key);
                        string resultStr = client.IncomeWeiXin(xmlStr);

                        var resultEntity = XmlEntityExchange<returnUnifyOrder>.ConvertXmlToEntity(resultStr);
                        #endregion
                        if (resultEntity != null && !string.IsNullOrEmpty(resultEntity.prepay_id))
                        {
                            JsIncomeModel q = new JsIncomeModel();
                            q.appId = ConfigurationManager.AppSettings["AppID"].ToString();
                            q.timeStamp = JsIncomHelp.GetTimeStamp();
                            q.nonceStr = JsIncomHelp.GetRandCode(32);
                            q.package = "prepay_id=" + resultEntity.prepay_id;
                            q.signType = "MD5";
                            q.paySign = new JsIncomHelp().DoDataForsign(q, key);
                            ViewBag.entity = q;
                        }

                        ViewBag.body = body;
                        ViewBag.total_fee = 0.01;//total_fee  先写死，要不然支付不起;
                        ViewBag.orderno = orderno;
                        ViewBag.openid = openId;
                        BLL.BackgroundUserBll_log.AddLog("userpay", resultStr, Request.UserHostAddress);
                    }
                    catch (Exception ex)
                    {
                        BLL.BackgroundUserBll_log.AddLog("userpay", "微信公共支付有问题！错误：" + ex.Message, Request.UserHostAddress);
                        return null;
                    }
                    #endregion
                }
                return View(outModel);
            }
            catch (Exception e)
            {
                BLL.BackgroundUserBll_log.AddLog("标记120", "支付时发生：" + e.Message, "0.0.0.0");
                return null;
            }
        }
        //说实话我也不知道这个是用来干什么的
        public ActionResult notify()
        {
            try
            {
                string AppID = ConfigurationManager.AppSettings["AppID"];
                StreamReader reader = new StreamReader(Request.InputStream);
                string xml = reader.ReadToEnd();
                reader.Close();
                BLL.BackgroundUserBll_log.AddLog("我就是想证明进来了么", xml, Request.UserHostAddress);
            }
            catch (Exception ex)
            {
                //CommonMethod.WriteTo_Txt("notify" + ex.Message);
            }
            return Content("success");
        }
    }
}
