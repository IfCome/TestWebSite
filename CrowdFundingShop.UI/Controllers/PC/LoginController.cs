using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrowdFundingShop.BLL;
using CrowdFundingShop.Model;
using CrowdFundingShop.Utility;

namespace CrowdFundingShop.UI.Controllers.PC
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string Login(string UserName, string Pwd)
        {
            string result = "用户名或密码错误";
            Model.BackgroundUserInfo userInfo = BLL.BackgroundUserBll.Login(UserName, Pwd);
            if (userInfo != null)
            {
                Security.SetUserLoginCookies(userInfo.ID.ToString(), this.Response);
                result = "ok";
            }
            return result;
        }

    }
}
