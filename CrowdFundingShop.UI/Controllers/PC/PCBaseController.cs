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
        //protected SellerUserInfo LoginUserInfo = new SellerUserInfo();
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
                    //TODO: 获取当前用户信息
                    //LoginUserInfo = UserInfoService.GetSingleById(userId);
                    //Identity.User = new CurrentUserInfo()
                    //{
                    //    UserId=Guid.NewGuid(),
                    //    UserName="asdfas"
                    //};
                    //Identity.Shop=new CurrentShopInfo(){
                    //    ShopId=Guid.NewGuid(),
                    //    ShopName="fadsfa",
                    //};

                    Security.SetUserLoginCookies(userId, this.Response);


                    //TODO: 校验用户是否有当前页面操作权限
                    // CheckAction(filterContext);

                    return;
                }
            }
            // 没有登录成功者返回登录页面
            filterContext.Result = Redirect(Url.Action("Index", "Login"));
            return;
        }

        //private void CheckAction(AuthorizationContext filterContext)
        //{
        //    // 后门 管理员admin具有所有权限
        //    if (LoginUserInfo.UserName.ToLower().Equals("admin"))
        //    {
        //        return;
        //    }

        //    // 获取用户请求的控制器名称和行为名称
        //    string controllerName = RouteData.GetRequiredString("controller");
        //    string actionName = RouteData.GetRequiredString("action");

        //    // 判断该行为是否被删除
        //    ActionInfo actionInfo = ActionInfoService.GetList(a =>
        //         a.ActionName.ToLower() == actionName.ToLower()
        //         && a.ControllerName.ToLower() == controllerName.ToLower()
        //         && a.IsDelete == false).FirstOrDefault();
        //    if (actionInfo == null)
        //    {
        //        // 没有查询到数据，跳转到提示页面
        //        filterContext.Result = new RedirectResult("/ErrorPage.html");
        //        return;
        //    }

        //    // 通过行为表 判断用户是否具有该行为权限
        //    UserAction userAction = UserActionService.GetList(ua =>
        //        ua.UserId == LoginUserInfo.UserId
        //        && ua.ActionId == actionInfo.ActionId).FirstOrDefault();
        //    if (userAction != null)
        //    {
        //        // 否决表中存在该用户和行为的描述
        //        if (userAction.IsAllow == true)
        //        {
        //            // 允许该行为 结束该方法
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        // 否决表中不存在该用户和行为的描述，则判断用户角色是否具有该行为权限
        //        var re2 = from r in LoginUserInfo.RoleInfo
        //                  from a in r.ActionInfo
        //                  where a.ActionId == actionInfo.ActionId
        //                  select a;
        //        if (re2.Any())
        //        {
        //            // 查询到该权限，结束方法
        //            return;
        //        }
        //    }
        //    // 用户没有该权限,跳转到无权限页面
        //    filterContext.Result = new RedirectResult("/NoAuthority.html");
        //}
    }
}
