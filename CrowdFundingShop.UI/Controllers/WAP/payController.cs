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

        //        /// <summary>
        //        /// 公共确认订单
        //        /// </summary>
        //        /// <returns></returns>
        //        public ActionResult NotarizePayOrder()
        //        {
        //            string id = Request.QueryString["id"];
        //            string url = Request.QueryString["url"];
        //            return Redirect(ViewBag.url);
        //        }


        /// <summary>
        /// 微信公共支付
        /// </summary>
        /// <returns></returns>
        public ActionResult userpay()
        {
            string openId = "ooqpuwaNp_FClIjYwEBe0DjdvTME";//Request.QueryString["openid"].ToString();

            try
            {
                string body = "Ipad mini  16G  白色";
                int total_fee = 100;
                string orderno = "20160511105602";

                #region 向微信下订单
                WxIncomeHelp client = new WxIncomeHelp();
                UnifyOrder entity = new UnifyOrder();
                entity.appid = ConfigurationManager.AppSettings["AppID"].ToString();
                entity.mch_id = ConfigurationManager.AppSettings["mch_id"].ToString();
                //随机字符串不长于32位
                entity.nonce_str =GetRandCode(32);
                //商户订单号32个字符内、可包含字母
                entity.out_trade_no = orderno;
                entity.body = body;
                entity.total_fee = total_fee;
                entity.spbill_create_ip = "101.201.142.71";
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
                    q.timeStamp = "";// GetDateTimeStamp();
                    q.nonceStr = GetRandCode(32);
                    q.package = "prepay_id=" + resultEntity.prepay_id;
                    q.signType = "MD5";
                    q.paySign = new JsIncomHelp().DoDataForsign(q, key);
                    ViewBag.entity = q;
                }

                ViewBag.body = body;
                ViewBag.total_fee = total_fee / 100;
                ViewBag.orderno = orderno;
                ViewBag.openid = openId;
            }
            catch (Exception ex)
            {
                BLL.BackgroundUserBll_log.AddLog("userpay", "微信公共支付有问题！错误：" + ex.Message, Request.UserHostAddress);
                return null;
            }
            return View();
        }

        /// <summary>
        /// 公共支付成功返回页面
        /// </summary>
        /// <returns></returns>
        public ActionResult PayAction()
        {
            return Redirect("/user/orders");
            //return View();
        }

        //public ActionResult notify()
        //{
        //    try
        //    {
        //        string AppID = ConfigurationManager.AppSettings["AppID"];
        //        StreamReader reader = new StreamReader(Request.InputStream);
        //        string xml = reader.ReadToEnd();
        //        reader.Close();
        //        var resultEntity = XmlEntityExchange<PayNotify>.ConvertXmlToEntity(xml);
        //        ViewModelOrder ordermodel = new ViewModelOrder();
        //        using (OrderServiceClient client = new OrderServiceClient())
        //        {
        //            ordermodel = client.GetByOrderNo(resultEntity.out_trade_no);
        //            int ispay = Convert.ToInt32(ordermodel.IsPay);
        //            if (ordermodel != null && ordermodel.ID > 0 && ispay == 0)
        //            {
        //                ordermodel.IsPay = 1;
        //                ordermodel.PayTime = DateTime.Now;
        //                ordermodel.PayType = 8;//微信支付
        //                int result = client.Update(ordermodel);

        //                //分销订单
        //                if ((ordermodel.OrderType == 7 || ordermodel.OrderType == 8) && result > 0)
        //                {
        //                    ViewModelTOCUser fromuser = new ViewModelTOCUser();
        //                    using (TOCUserServiceClient service = new TOCUserServiceClient())
        //                    {
        //                        fromuser = service.GetById(Convert.ToInt32(ordermodel.UserId));
        //                    }
        //                    string openid = "";
        //                    string opneids = fromuser.WXOpenIds;
        //                    string[] openidsarray = opneids.Split(',');
        //                    foreach (var a in openidsarray)
        //                    {
        //                        if (a.Split('#')[0] == AppID)
        //                            openid = a.Split('#')[1];
        //                    }

        //                    SqlParameter[] params_arc ={
        //                        new SqlParameter("@AppKey",SqlDbType.NVarChar),
        //                        new SqlParameter("@OrderID",SqlDbType.Int,4),
        //                        new SqlParameter("@OrderNo",SqlDbType.NVarChar),
        //                        new SqlParameter("@ActivityProductID",SqlDbType.Int,4),
        //                        new SqlParameter("@ActivityProductName",SqlDbType.NVarChar),
        //                        new SqlParameter("@OrderAmount",SqlDbType.Decimal),
        //                        new SqlParameter("@FromUserID",SqlDbType.Int,4),
        //                        new SqlParameter("@FromUserSex",SqlDbType.Int,4),
        //                        new SqlParameter("@FromOpenID",SqlDbType.NVarChar),
        //                        new SqlParameter("@FromUserLevel",SqlDbType.Int,4),
        //                        new SqlParameter("@FromNickName",SqlDbType.NVarChar)
        //                    };

        //                    params_arc[0].Value = AppID;
        //                    params_arc[1].Value = ordermodel.ID;
        //                    params_arc[2].Value = ordermodel.OrderNo;
        //                    params_arc[3].Value = ordermodel.ActivityProductID;
        //                    params_arc[4].Value = ordermodel.ProductName;
        //                    params_arc[5].Value = ordermodel.TotalFee;
        //                    params_arc[6].Value = fromuser.ID;
        //                    params_arc[7].Value = fromuser.Sex == null ? 0 : Convert.ToInt32(fromuser.Sex);
        //                    params_arc[8].Value = openid;
        //                    params_arc[9].Value = fromuser.Level == null ? 0 : fromuser.Level;
        //                    params_arc[10].Value = fromuser.LoginName;
        //                    DataSet dsinfo = BIStone.Data.SurveyDbHelper.ExecuteDataset("pro_AddBackCashRecords", params_arc);

        //                    //返现通知
        //                    if (dsinfo != null && dsinfo.Tables[0].Rows.Count > 0)
        //                    {

        //                        for (int i = 0; i < dsinfo.Tables[0].Rows.Count; i++)
        //                        {
        //                            string msgopenid = dsinfo.Tables[0].Rows[i]["openid"].ToString();
        //                            string msgcontent = dsinfo.Tables[0].Rows[i]["msgcontent"].ToString();
        //                            string content = ReturnJson.ReturnMessage(ReturnJson.ReText(msgopenid, msgcontent), AccessTokenStr);
        //                        }
        //                    }

        //                    int coupon = 0;
        //                    int amount = 0;
        //                    string coupons = "";
        //                    //生成专车优惠券
        //                    if (ordermodel.TotalFee == 200)
        //                    {
        //                        coupon = 3;
        //                        amount = 100;
        //                    }
        //                    if (ordermodel.TotalFee == 400)
        //                    {
        //                        coupon = 3;
        //                        amount = 200;
        //                    }
        //                    if (ordermodel.TotalFee == 800)
        //                    {
        //                        coupon = 6;
        //                        amount = 200;
        //                    }
        //                    for (int i = 0; i < coupon; i++)
        //                    {
        //                        string couponno = GetTimeStamp();
        //                        System.Random random = new Random(Guid.NewGuid().GetHashCode());
        //                        for (int c = 0; c < 4; c++)
        //                        {
        //                            couponno += random.Next(10).ToString();
        //                        }
        //                        if (coupons == "")
        //                            coupons = couponno;
        //                        else
        //                            coupons += "," + couponno;
        //                    }

        //                    SqlParameter[] paramsc ={
        //                        new SqlParameter("@ActivityProductID",SqlDbType.Int,4),
        //                        new SqlParameter("@userid",SqlDbType.Int,4),
        //                        new SqlParameter("@coupons",SqlDbType.NVarChar),
        //                        new SqlParameter("@Amount",SqlDbType.Int,4)
        //                    };

        //                    paramsc[0].Value = ordermodel.ActivityProductID;
        //                    paramsc[1].Value = ordermodel.UserId;
        //                    paramsc[2].Value = coupons;
        //                    paramsc[3].Value = amount;
        //                    BIStone.Data.SurveyDbHelper.ExecuteDataset("pro_ADProductCoupons", paramsc);
        //                }
        //            }
        //            else
        //            {
        //                BIStone.Data.SurveyDbHelper.ExecuteScalar("update WinningInfo set Status = 1 where scene_str ='" + resultEntity.out_trade_no + "'");
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //CommonMethod.WriteTo_Txt("notify" + ex.Message);
        //    }
        //    return Content("success");
        //}

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
//        /// <summary>
//        /// 根据订单Id获取订单信息
//        /// 根据业务类型获取返回的配置好的返回地址
//        /// </summary>
//        /// <param name="id">订单编号</param>
//        /// <param name="btype">业务类型</param>
//        /// <param name="url">返回地址</param>
//        /// <returns></returns>
//        public ViewModelOrder GetData(string id, string btype)
//        {
//            var obj = new ViewModelOrder();
//            if (!string.IsNullOrEmpty(id))
//            {
//                #region 获取订单相关信息
//                using (OrderServiceClient client = new OrderServiceClient())
//                {
//                    var entity = client.GetById(Convert.ToInt32(id));
//                    if (entity != null)
//                        obj = entity;
//                    else
//                        obj = null;
//                }
//                #endregion
//            }
//            else
//            {
//                obj = null;
//            }
//            return obj;
//        }


//    }
//}
