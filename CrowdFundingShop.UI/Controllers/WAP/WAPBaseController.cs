using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CrowdFundingShop.Utility;

namespace CrowdFundingShop.UI.Controllers.WAP
{
    public class WAPBaseController : Controller
    {
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Session["user"] != null)
            {
                // 获取用户的Session信息
                string userInfoSession = Session["user"].ToString();
                // 解析用户信息
                var Jss = new JavaScriptSerializer();
                var userData = (Dictionary<string, object>)Jss.DeserializeObject(userInfoSession);

                // 查询库中当前用户的信息
                Model.ConsumerInfo currentConsumer = BLL.ConsumerInfoBll.GetByWeiXinAccount(userData["openid"].ToString());
                if (currentConsumer == null)
                {
                    // 没有当前用户信息，则重新登录
                    filterContext.Result = Redirect(Config.OauthUrl + "?backurl=" + Request.Url.AbsoluteUri);
                }
                else
                {
                    Identity.LoginConsumer = new Model.ConsumerInfo()
                    {
                        ID=currentConsumer.ID,
                        WeiXinAccount = userData["openid"].ToString(),
                        NickName = userData["nickname"].ToString(),
                        Address = userData["country"].ToString() + userData["province"].ToString() + userData["city"].ToString(),
                        HeadIcon = userData["headimgurl"].ToString()
                    };
                }
            }
            else
            {
                // 没有登录成功者返回登录页面
                filterContext.Result = Redirect(Config.OauthUrl + "?backurl=" + Request.Url.AbsoluteUri);
            }
            return;
        }
    }
}
