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
    public class OauthController : Controller
    {
        //
        // GET: /Oauth/
        JavaScriptSerializer Jss = new JavaScriptSerializer();
        public ActionResult Index()
        {
            return View();
        }

        //用户中心																														
        public ActionResult usercenter()
        {
            try
            {
                //Session.Clear();																														
                if ((Session["userid"] == null) && String.IsNullOrEmpty(Request.QueryString["unionid"]))
                {
                    var AppID = ConfigurationManager.AppSettings["AppID"];
                    var domainurl = ConfigurationManager.AppSettings["domainurl"];
                    var red_url = Url.Encode(domainurl + "/oauth/usercenter");
                    var oauth_url = ConfigurationManager.AppSettings["oauth_url"];
                    return Redirect(oauth_url + "?backurl=" + red_url);
                }
                else
                {
                    return null;
                    //具体逻辑  用户信息写入数据库并且跳转至商品页
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        //微信用户授权																													
        public ActionResult userlogin(string backurl)
        {
            var AppID = ConfigurationManager.AppSettings["AppID"];
            var domainurl = ConfigurationManager.AppSettings["domainurl"];
            var red_url = Url.Encode(domainurl + "/oauth/getuserwechatinfo");
            const string scope = "snsapi_userinfo";
            Session["backurl"] = backurl;
            var state = GetRandCode(8);
            Session[state] = state;

            return Redirect(GetCodeUrl(AppID, red_url, scope, state));
        }
        public ActionResult getuserwechatinfo(string state, string code)
        {
            BLL.BackgroundUserBll_log.AddLog("微信获取用户", "state：" + state + "；" + "code：" + code, "0.0.0.0");
            var AppID = ConfigurationManager.AppSettings["AppID"];
            var AppSecret = ConfigurationManager.AppSettings["AppSecret"];
            string userString = "";
            if (Session["user"] == null)
            {
                userString = GetUserInfo(AppID, AppSecret, code);
                Session["user"] = userString;
                return RedirectToAction("SaveUser");
            }
            else
            {
                string url = Session["backurl"].ToString();
                userString = Session["user"].ToString();
                var Jss = new JavaScriptSerializer();
                var data = (Dictionary<string, object>)Jss.DeserializeObject(userString);
                if (data == null || data["openid"] == null)
                    return Redirect("~/htmls/error.htm");
                if (url.Contains("?"))
                    return Redirect(url + "&unionid=" + data["unionid"].ToString());
                else
                    return Redirect(url + "?unionid=" + data["unionid"].ToString());
            }
        }

        /// <summary>
        /// 对页面是否要用授权 
        /// </summary>
        /// <param name="Appid">微信应用id</param>
        /// <param name="redirect_uri">回调页面</param>
        /// <param name="scope">应用授权作用域snsapi_base（不弹出授权页面，直接跳转，只能获取用户openid），snsapi_userinfo （弹出授权页面，可通过openid拿到昵称、性别、所在地。并且，即使在未关注的情况下，只要用户授权，也能获取其信息）</param>
        /// state is a random string
        /// <returns>授权地址</returns>
        public string GetCodeUrl(string Appid, string redirect_uri, string scope, string state)
        {
            return
                string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&state={3}#wechat_redirect", Appid, redirect_uri, scope, state);
        }
        /// <summary>
        ///用code换取获取用户信息（包括非关注用户的）
        /// </summary>
        /// <param name="Appid"></param>
        /// <param name="Appsecret"></param>
        /// <param name="Code">回调页面带的code参数</param>
        /// <returns>获取用户信息（json格式）</returns>
        public string GetUserInfo(string Appid, string Appsecret, string Code)
        {
            try
            {
                var url = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", Appid, Appsecret, Code);
                var ReText = WebRequestPostOrGet(url, "");
                var DicText = (Dictionary<string, object>)Jss.DeserializeObject(ReText);
                if (!DicText.ContainsKey("openid"))
                {
                    //WriteTxt_Log("获取openid失败，错误码：" + DicText["errcode"].ToString());
                    return "";
                }
                else
                {
                    return WebRequestPostOrGet("https://api.weixin.qq.com/sns/userinfo?access_token=" + DicText["access_token"] + "&openid=" + DicText["openid"] + "&lang=zh_CN", "");
                }
            }
            catch (Exception ex)
            {
                //WriteTxt_Log("ex:" + ex.Message);
                return "";
                //throw;																												
            }
        }

        #region 生成随机字符
        /// <summary>																												
        /// 生成随机字符																												
        /// </summary>																												
        /// <param name="iLength">生成字符串的长度</param>																												
        /// <returns>返回随机字符串</returns>																												
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
        #endregion

        #region Post/Get提交调用抓取
        /// <summary>																												
        /// Post/get 提交调用抓取																												
        /// </summary>																												
        /// <param name="url">提交地址</param>																												
        /// <param name="param">参数</param>																												
        /// <returns>string</returns>																												
        public static string WebRequestPostOrGet(string sUrl, string sParam)
        {
            try
            {
                Uri uriurl = new Uri(sUrl);
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uriurl);//HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url + (url.IndexOf("?") > -1 ? "" : "?") + param);																												
                req.Method = "Post";
                req.Timeout = 120 * 1000;
                req.ContentType = "application/x-www-form-urlencoded;";
                if (!string.IsNullOrEmpty(sParam))
                {
                    byte[] bt = System.Text.Encoding.UTF8.GetBytes(sParam);
                    req.ContentLength = bt.Length;
                    using (Stream reqStream = req.GetRequestStream())//using 使用可以释放using段内的内存																												
                    {
                        reqStream.Write(bt, 0, bt.Length);
                        reqStream.Flush();
                    }
                }

                using (WebResponse res = req.GetResponse())
                {
                    //在这里对接收到的页面内容进行处理 																												
                    Stream resStream = res.GetResponseStream();
                    StreamReader resStreamReader = new StreamReader(resStream, System.Text.Encoding.UTF8);
                    string resLine;
                    System.Text.StringBuilder resStringBuilder = new System.Text.StringBuilder();
                    while ((resLine = resStreamReader.ReadLine()) != null)
                    {
                        resStringBuilder.Append(resLine + System.Environment.NewLine);
                    }
                    resStream.Close();
                    resStreamReader.Close();
                    return resStringBuilder.ToString();
                }
            }
            catch (Exception ex)
            {
                //WriteTxt_Log(ex);																												
                return ex.Message;//url错误时候回报错																												
            }
        }
        #endregion

        #region 保存用户信息到服务器
        public ActionResult SaveUser()
        {
            string outModel = Session["user"].ToString();
            return View("~/Views/GoodsList/List.cshtml?userinfo=" + outModel);
        }
        #endregion
    }
}