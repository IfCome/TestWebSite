using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Web.Script.Serialization;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using CrowdFundingShop.Model;
using CrowdFundingShop.Utility;

namespace CrowdFundingShop.UI.Controllers.WAP
{
    public class payController : WAPBaseController
    {
        public ActionResult userpay(string shoppingcartid = "")
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
                        string body = string.Empty;
                        int total_fee = 0;
                        foreach (var item in outModel)
                        {
                            body += item.GoodsName + "X" + item.StoreCount + "；";
                            total_fee += Converter.TryToInt32(item.StoreCount) * 100;
                        }
                        //string body = "测试机";
                        /*******************************************测试测试测试*************************************************/
                        total_fee = 1;
                        /*******************************************测试测试测试*************************************************/
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
                        ViewBag.total_fee = total_fee / 100;//total_fee  先写死，要不然支付不起;
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
        public ActionResult orderDetail()
        {
            Response.Write("orderDetail");
            Response.End();
            return View();
        }

        public static string GetRandCode(int iLength)
        {
            string sCode = "";
            if (iLength == 0)
            {
                iLength = 4;
            }
            string codeSerial = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            string[] arr = codeSerial.Split(',');
            int randValue = -1;
            Random rand = new Random(unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < iLength; i++)
            {
                randValue = rand.Next(0, arr.Length - 1);
                sCode += arr[randValue];
            }
            return sCode;
        }
    }
}