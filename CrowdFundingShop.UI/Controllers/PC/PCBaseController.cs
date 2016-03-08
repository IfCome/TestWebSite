using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrowdFundingShop.Utility;

namespace CrowdFundingShop.UI.Controllers.PC
{
    public class PCBaseController : Controller
    {
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            // 获取用户的 cookie 对象
            HttpCookie userIdCookie = Request.Cookies["ID"];
            HttpCookie pairACookie = Request.Cookies["PairA"];
            HttpCookie pairBCookie = Request.Cookies["PairB"];

            if (userIdCookie != null && pairACookie != null && pairBCookie != null)
            {
                // 解密获取用户ID
                string userId = Security.Decrypt(userIdCookie.Value);
                string pairA = pairACookie.Value;
                string pairB = pairBCookie.Value;

                // 通过pairA、pairB是否匹配判断用户是否登录
                if (!string.IsNullOrEmpty(userId) && Security.Encrypt(userId + pairA).Equals(pairB))
                {
                    // 获取当前用户信息
                    Identity.LoginUserInfo = BLL.BackgroundUserBll.GetLoginUserInfo(Converter.TryToInt64(userId));
                    if (Identity.LoginUserInfo != null)
                    {
                        Security.SetUserLoginCookies(userId, this.Response);
                        return;
                    }
                }
            }
            // 没有登录成功者返回登录页面
            filterContext.Result = Redirect(Url.Action("Index", "Login"));
            return;
        }
    }
}
