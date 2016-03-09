using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrowdFundingShop.UI.Controllers.PC
{
    public class UserInfoController : PCBaseController
    {
        //
        // GET: /UserInfo/

        // 用户管理页面
        public ActionResult IndexPage()
        {
            return View();
        }

        // 添加用户页面
        public ActionResult AddUser()
        {
            return View();
        }

        // 提交用户信息
        public ActionResult AddUserCallBack(Models.PC.BackGroundUserInfoModelIn InModel)
        {
            string errorType = "";
            string msg = "OK";
            // 验证参数
            if (string.IsNullOrWhiteSpace(InModel.UserName))
            {
                errorType = "UserName";
                msg = "请输入用户名";
            }
            else if (false)
            {
                errorType = "UserName";
                msg = "该用户名已存在";
            }

            else if (string.IsNullOrWhiteSpace(InModel.Password))
            {
                errorType = "Password";
                msg = "请输入密码";
            }
            else if (InModel.Password != InModel.RePassword)
            {
                errorType = "RePassword";
                msg = "两次输入的密码不一致";
            }

            return Json(new { Message = msg, ErrorType = errorType });
        }
    }
}
