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
using System.Xml;

namespace CrowdFundingShop.UI.Controllers.WAP
{
    public class OauthController : Controller
    {
        //
        // GET: /Oauth/
        JavaScriptSerializer Jss = new JavaScriptSerializer();

        //public OauthController()
        //{
        //    usercenter();
        //}

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
            var red_url = Url.Encode(domainurl + "/Oauth/getuserwechatinfo");
            const string scope = "snsapi_userinfo";
            Session["backurl"] = backurl;
            var state = GetRandCode(8);
            Session[state] = state;
            return Redirect(GetCodeUrl(AppID, red_url, scope, state));
        }
        public ActionResult getuserwechatinfo(string state, string code)
        {
            try
            {
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
                        return View("~/Views/GoodsList/List.cshtml?userinfo=错误0");
                    if (url.Contains("?"))
                        return Redirect(url + "&unionid=" + data["openid"].ToString());
                    else
                        return Redirect(url + "?unionid=" + data["openid"].ToString());
                }
            }
            catch (Exception e)
            {
                BLL.BackgroundUserBll_log.AddLog("错误了", e.Message, "0.0.0.0");
                return View("~/Views/GoodsList/List.cshtml?userinfo=错误2");
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
            return string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&state={3}&connect_redirect=1#wechat_redirect", Appid, redirect_uri, scope, state);
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
                    BLL.BackgroundUserBll_log.AddLog("标记111", "获取openid失败，错误码：" + DicText["errcode"].ToString(), "0.0.0.0");
                    return "";
                }
                else
                {
                    return WebRequestPostOrGet("https://api.weixin.qq.com/sns/userinfo?access_token=" + DicText["access_token"] + "&openid=" + DicText["openid"] + "&lang=zh_CN", "");
                }
            }
            catch (Exception ex)
            {
                BLL.BackgroundUserBll_log.AddLog("标记112", "ex：" + ex.Message, "0.0.0.0");
                //WriteTxt_Log("ex:" + ex.Message);
                return "";
                //throw;																												
            }
        }


        /// <summary>
        /// 判断access_token是否过期
        /// </summary>
        /// <returns></returns>
        public string GetExistAccessToken(Dictionary<string, object> DicText)
        {
            // 读取XML文件中的数据
            string filepath = Server.MapPath("/File/XMLToken.xml");
            StreamReader str = new StreamReader(filepath, System.Text.Encoding.UTF8);
            XmlDocument xml = new XmlDocument();
            xml.Load(str);
            str.Close();
            str.Dispose();
            string Token = xml.SelectSingleNode("xml").SelectSingleNode("AccessToken").InnerText;
            DateTime AccessExpires = Convert.ToDateTime(xml.SelectSingleNode("xml").SelectSingleNode("AccessExpires").InnerText);

            if (DateTime.Now >= AccessExpires)
            {
                xml.SelectSingleNode("xml").SelectSingleNode("AccessToken").InnerText = DicText["access_token"].ToString();
                DateTime _accessExpires = DateTime.Now.AddSeconds(Converter.TryToInt32((DicText["expires_in"])));
                xml.SelectSingleNode("xml").SelectSingleNode("AccessExpires").InnerText = _accessExpires.ToString();
                xml.Save(filepath);
                Token = DicText["access_token"].ToString();
            }
            return Token;
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
                BLL.BackgroundUserBll_log.AddLog("标记112", "错误内容：" + ex.Message, "0.0.0.0");
                return ex.Message;//url错误时候回报错
            }
        }
        #endregion

        #region 保存用户信息到服务器
        public ActionResult SaveUser()
        {
            try
            {
                string userString = Session["user"].ToString();
                var data = (Dictionary<string, object>)Jss.DeserializeObject(userString);
                Model.ConsumerInfo consumerInfo = new Model.ConsumerInfo()
                {
                    WeiXinAccount = data["openid"].ToString(),
                    NickName = data["nickname"].ToString(),
                    Address = data["country"].ToString() + data["province"].ToString() + data["city"].ToString(),
                    HeadIcon = data["headimgurl"].ToString()
                };
                Model.ConsumerInfo currentConsumer = BLL.ConsumerInfoBll.GetByWeiXinAccount(data["openid"].ToString());
                if (currentConsumer == null)
                {
                    string consumerid = BLL.ConsumerInfoBll.Add(consumerInfo);
                    if (!string.IsNullOrEmpty(consumerid))
                    {
                        consumerInfo.ID = Converter.TryToInt64(consumerid);
                        Identity.LoginConsumer = consumerInfo;
                    }
                    else
                    {
                        return View(ConfigurationManager.AppSettings["domainurl"] + "/oauth/usercenter");
                    }
                }
                else
                {
                    consumerInfo.ID = currentConsumer.ID;
                    Identity.LoginConsumer = consumerInfo;
                }
            }
            catch (Exception e)
            {
                BLL.BackgroundUserBll_log.AddLog("标记119", "存用户时发生错误：" + e.Message, "0.0.0.0");
            }
            return View("~/Views/GoodsList/List.cshtml");
        }
        #endregion


        #region 支付相关

        #endregion










        /// <summary>
        /// 根据当前日期 判断Access_Token 
        /// 是否超期  
        /// 如果超期返回新的Access_Token   
        /// 否则返回之前的Access_Token
        /// </summary>
        /// <param name="AppID"></param>
        /// <param name="AppSecret"></param>
        /// <returns></returns>
        public static string IsExistAccess_Token(string AppID, string AppSecret, out DateTime? expiresDate)
        {
            expiresDate = null;
            string Token = string.Empty;
            string ExpiresDateStr = string.Empty;
            string expires_in = string.Empty;
            try
            {

                // 读取XML文件中的数据，并显示出来 ，注意文件路径
                string filepath = HttpContext.Current.Server.MapPath("~/configs/CommonVariable.xml");
                StreamReader str = new StreamReader(filepath, System.Text.Encoding.UTF8);
                XmlDocument xml = new XmlDocument();
                xml.Load(str);
                str.Close();
                str.Dispose();
                Token = xml.SelectSingleNode("Variable").SelectSingleNode("Wx_Access_Token").InnerText;
                ExpiresDateStr = xml.SelectSingleNode("Variable").SelectSingleNode("Wx_Access_Token_Expires").InnerText;

                //过期时间为空，初始化数据
                if (!string.IsNullOrEmpty(ExpiresDateStr))
                {
                    DateTime ExpiresDate = Convert.ToDateTime(ExpiresDateStr);
                    //是否过期,过期就需要重新获取数据并更新
                    if (DateTime.Compare(DateTime.Now, ExpiresDate) > 0)
                    {
                        Token = GetAccessToken(AppID, AppSecret, out expires_in);
                        DateTime _ExpiresDate = DateTime.Now;
                        xml.SelectSingleNode("Variable").SelectSingleNode("Wx_Access_Token").InnerText = Token;
                        _ExpiresDate = _ExpiresDate.AddSeconds(int.Parse(expires_in) - 200);//int.Parse(expires_in) - 200
                        expiresDate = Convert.ToDateTime(_ExpiresDate);
                        xml.SelectSingleNode("Variable").SelectSingleNode("Wx_Access_Token_Expires").InnerText = _ExpiresDate.ToString();
                        CommonMethod.WriteTo_Txt("Save");
                        xml.Save(filepath);
                    }
                }
                else
                {
                    Token = GetAccessToken(AppID, AppSecret, out expires_in);
                    CommonMethod.WriteTo_Txt("Save" + Token);
                    DateTime _ExpiresDate = DateTime.Now;
                    xml.SelectSingleNode("Variable").SelectSingleNode("Wx_Access_Token").InnerText = Token;
                    _ExpiresDate = _ExpiresDate.AddSeconds(int.Parse(expires_in) - 200);//
                    expiresDate = Convert.ToDateTime(_ExpiresDate);
                    xml.SelectSingleNode("Variable").SelectSingleNode("Wx_Access_Token_Expires").InnerText = _ExpiresDate.ToString();
                    xml.Save(filepath);
                }
            }
            catch (Exception ex)
            {
                CommonMethod.WriteTo_Txt("IsExistAccess_Token:" + ex.Message);
            }
            return Token;
        }

    }
}