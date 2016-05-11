using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using CrowdFundingShop.Model;

namespace CrowdFundingShop.Utility
{
    public class WxIncomeHelp
    {
        /// <summary>
        /// 调用微信收入接口前处理数据，包括sign验证等
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string DoDataForIncomeWeiXin(UnifyOrder entity, string key)
        {

            string postData = @"<xml> 
                                 <appid>{0}</appid> 
                                 <mch_id>{1}</mch_id> 
                                 <nonce_str>{2}</nonce_str> 
                                 <body>{3}</body> 
                                 <out_trade_no>{4}</out_trade_no> 
                                 <total_fee>{5}</total_fee> 
                                 <spbill_create_ip>{6}</spbill_create_ip> 
                                 <notify_url>{7}</notify_url> 
                                 <trade_type>{8}</trade_type>";
            postData = string.Format(postData,
                                            entity.appid,
                                            entity.mch_id,
                                            entity.nonce_str,
                                            entity.body,
                                            entity.out_trade_no,
                                            entity.total_fee,
                                            entity.spbill_create_ip,
                                            entity.notify_url,
                                            entity.trade_type
                                );


            //原始传入参数
            string[] signTemp = { "appid=" + entity.appid, "mch_id=" + entity.mch_id, "nonce_str=" + entity.nonce_str, "body=" + entity.body, "out_trade_no=" + entity.out_trade_no, "total_fee=" + entity.total_fee, "spbill_create_ip=" + entity.spbill_create_ip, "notify_url=" + entity.notify_url, "trade_type=" + entity.trade_type };
            List<string> signList = signTemp.ToList();
            //拼接原始字符串
            if (!string.IsNullOrEmpty(entity.device_info))
            {
                postData += "<device_info>{0}</device_info> ";
                postData = string.Format(postData, entity.device_info);
                signList.Add("device_info=" + entity.device_info);
            }
            if (!string.IsNullOrEmpty(entity.detail))
            {
                postData += "<detail>{0}</detail> ";
                postData = string.Format(postData, entity.detail);
                signList.Add("detail=" + entity.detail);
            }
            if (!string.IsNullOrEmpty(entity.attach))
            {
                postData += "<attach>{0}</attach> ";
                postData = string.Format(postData, entity.attach);
                signList.Add("attach=" + entity.attach);
            }
            if (!string.IsNullOrEmpty(entity.fee_type))
            {
                postData += "<fee_type>{0}</fee_type> ";
                postData = string.Format(postData, entity.fee_type);
                signList.Add("fee_type=" + entity.fee_type);
            }

            if (!string.IsNullOrEmpty(entity.time_start))
            {
                postData += "<time_start>{0}</time_start> ";
                postData = string.Format(postData, entity.time_start);
                signList.Add("time_start=" + entity.time_start);
            }

            if (!string.IsNullOrEmpty(entity.time_expire))
            {
                postData += "<time_expire>{0}</time_expire> ";
                postData = string.Format(postData, entity.time_expire);
                signList.Add("time_expire=" + entity.time_expire);
            }


            if (!string.IsNullOrEmpty(entity.goods_tag))
            {
                postData += "<goods_tag>{0}</goods_tag> ";
                postData = string.Format(postData, entity.goods_tag);
                signList.Add("goods_tag=" + entity.goods_tag);
            }

            if (!string.IsNullOrEmpty(entity.product_id))
            {
                postData += "<product_id>{0}</product_id> ";
                postData = string.Format(postData, entity.product_id);
                signList.Add("product_id=" + entity.product_id);
            }

            if (!string.IsNullOrEmpty(entity.limit_pay))
            {
                postData += "<limit_pay>{0}</limit_pay> ";
                postData = string.Format(postData, entity.limit_pay);
                signList.Add("limit_pay=" + entity.limit_pay);
            }

            if (!string.IsNullOrEmpty(entity.openid))
            {
                postData += "<openid>{0}</openid> ";
                postData = string.Format(postData, entity.openid);
                signList.Add("openid=" + entity.openid);
            }


            #region 处理支付签名
            //对signList按照ASCII码从小到大的顺序排序
            signList.Sort();

            string signOld = string.Empty;
            string payForWeiXinOld = string.Empty;
            int i = 0;
            foreach (string temp in signList)
            {
                signOld += temp + "&";
                i++;
            }
            signOld = signOld.Substring(0, signOld.Length - 1);
            //拼接Key
            signOld += "&key=" + key;
            //处理支付签名
            entity.sign = Encrypt(signOld).ToUpper();
            #endregion
            postData += "<sign>{0}</sign></xml>";
            postData = string.Format(postData, entity.sign);
            return postData;
        }


        /// <summary>
        /// 企业付款钱数据处理,包括sign验证等
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string DoDataForIncomeWeiXin(CompanyPayment entity, string key)
        {
            string postData = @"<xml> 
                                 <mch_appid>{0}</mch_appid> 
                                 <mchid>{1}</mchid> 
                                 <nonce_str>{2}</nonce_str> 
                                 <partner_trade_no>{3}</partner_trade_no> 
                                 <openid>{4}</openid> 
                                 <check_name>{5}</check_name> 
                                 <amount>{6}</amount> 
                                 <desc>{7}</desc> 
                                 <spbill_create_ip>{8}</spbill_create_ip>";
            postData = string.Format(postData,
                                            entity.mch_appid,
                                            entity.mchid,
                                            entity.nonce_str,
                                            entity.partner_trade_no,
                                            entity.openid,
                                            entity.check_name,
                                            entity.amount,
                                            entity.desc,
                                            entity.spbill_create_ip
                                );


            //原始传入参数
            string[] signTemp = { "mch_appid=" + entity.mch_appid, "mchid=" + entity.mchid, "nonce_str=" + entity.nonce_str, "partner_trade_no=" + entity.partner_trade_no, "openid=" + entity.openid, "check_name=" + entity.check_name, "spbill_create_ip=" + entity.spbill_create_ip, "amount=" + entity.amount, "desc=" + entity.desc };
            List<string> signList = signTemp.ToList();
            //拼接原始字符串
            if (!string.IsNullOrEmpty(entity.device_info))
            {
                postData += "<device_info>{0}</device_info> ";
                postData = string.Format(postData, entity.device_info);
                signList.Add("device_info=" + entity.device_info);
            }
            if (!string.IsNullOrEmpty(entity.re_user_name))
            {
                postData += "<re_user_name>{0}</re_user_name> ";
                postData = string.Format(postData, entity.re_user_name);
                signList.Add("re_user_name=" + entity.re_user_name);
            }

            #region 处理支付签名
            //对signList按照ASCII码从小到大的顺序排序
            signList.Sort();

            string signOld = string.Empty;
            string payForWeiXinOld = string.Empty;
            int i = 0;
            foreach (string temp in signList)
            {
                signOld += temp + "&";
                i++;
            }
            signOld = signOld.Substring(0, signOld.Length - 1);
            //拼接Key
            signOld += "&key=" + key;
            //处理支付签名
            entity.sign = Encrypt(signOld).ToUpper();
            #endregion
            postData += "<sign>{0}</sign></xml>";
            postData = string.Format(postData, entity.sign);
            return postData;
        }

        /// <summary>
        /// 调用微信收入接口
        /// 统一下单接口
        /// </summary>
        /// <param name="payForWeiXin"></param>
        /// <returns></returns>
        public string IncomeWeiXin(string postData)
        {
            string result = null;
            try
            {
                result = PostPage("https://api.mch.weixin.qq.com/pay/unifiedorder", postData);
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        /// <summary>
        /// 企业付款调用
        /// </summary>
        /// <param name="postData"></param>
        /// <returns></returns>
        public string WithdrawDepositWeiXin(string postData)
        {
            string result = null;
            try
            {
                result = new WxPayForHelp().PostPage("https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers", postData);
            }
            catch (Exception ex)
            {
            }
            return result;

        }

        /// <summary>
        /// post微信请求
        /// </summary>
        /// <param name="posturl"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public string PostPage(string posturl, string postData)
        {
            Stream outstream = null;
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            byte[] data = encoding.GetBytes(postData);
            // 准备请求...  
            try
            {
                // 设置参数  
                request = WebRequest.Create(posturl) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = "text/xml";
                request.ContentLength = data.Length;
                outstream = request.GetRequestStream();
                outstream.Write(data, 0, data.Length);
                outstream.Close();
                //发送请求并获取相应回应数据  
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求  
                instream = response.GetResponseStream();
                sr = new StreamReader(instream, encoding);
                //返回结果网页（html）代码  
                string content = sr.ReadToEnd();
                string err = string.Empty;
                return content;

            }
            catch (Exception ex)
            {
                string err = ex.Message;
                return string.Empty;
            }
        }

        /// <summary>
        /// Md5加密
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String Encrypt(String s)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(s);
            bytes = md5.ComputeHash(bytes);
            md5.Clear();
            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
            }
            return ret.PadLeft(32, '0');
        }
    }
}
