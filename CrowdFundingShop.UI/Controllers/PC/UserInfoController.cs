﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrowdFundingShop.Utility;

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
            if (Identity.LoginUserInfo.RoleType != 10)
            {
                return Content("只有管理员才能添加用户");
            }
            return View();
        }

        // 提交用户信息
        [HttpPost]
        public ActionResult AddUserCallBack(Models.PC.BackGroundUserInfoModelIn InModel)
        {
            string errorType = "";
            string msg = "OK";
            if (Identity.LoginUserInfo.RoleType != 10)
            {
                errorType = "alert";
                msg = "只有管理员才能添加用户";
            }
            // 验证参数
            else if (string.IsNullOrWhiteSpace(InModel.UserName))
            {
                errorType = "UserName";
                msg = "请输入用户名";
            }
            else if (BLL.BackgroundUserBll.IsExistUserName(InModel.UserName))
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
            else
            {
                // 添加用户
                Model.BackgroundUserInfo userInfo = new Model.BackgroundUserInfo()
                {
                    UserName = InModel.UserName,
                    PassWord = Security.getMD5ByStr(InModel.Password),
                    RealName = InModel.RealName ?? "",
                    RoleType = Converter.TryToInt32(InModel.RoleType, 20),
                    Phone = InModel.Phone ?? "",
                    Email = InModel.Email ?? "",
                    QQ = InModel.QQ ?? "",
                    HeadIcon = InModel.HeadIcon ?? "",
                    CreateTime = DateTime.Now
                };
                if (BLL.BackgroundUserBll.AddUserInfo(userInfo) == false)
                {
                    errorType = "alert";
                    msg = "添加失败，请重试";
                }
                else
                {
                    // 记录日志
                    Model.BackgroundUserInfo_log logEntity = new Model.BackgroundUserInfo_log()
                    {
                        UserID = Identity.LoginUserInfo.ID,
                        OperateTile = "添加后台用户",
                        OperateDetail = string.Format("添加用户信息：用户名【{0}】，角色【{1}】", userInfo.UserName, userInfo.RoleType == 10 ? "管理员" : "普通用户"),
                        OperateTime = DateTime.Now,
                        IpAddress = Request.UserHostAddress
                    };
                    BLL.BackgroundUserBll_log.AddLog(logEntity);
                }
            }
            return Json(new { Message = msg, ErrorType = errorType });
        }


        public ActionResult GetUserInfoList(Models.PC.GetUserInfoListIn InModel)
        {
            InModel.PageSize = InModel.PageSize ?? 15;
            InModel.CurrentPage = InModel.CurrentPage ?? 1;
            InModel.RoleType = InModel.RoleType ?? 0;
            InModel.KeyWords = InModel.KeyWords ?? "";

            List<Model.BackgroundUserInfo> userInfoList = new List<Model.BackgroundUserInfo>();
            int allCount = 0;
            userInfoList = BLL.BackgroundUserBll.GetAllUserInfoList((int)InModel.PageSize, (int)InModel.CurrentPage, (int)InModel.RoleType, InModel.KeyWords, out allCount);
            return Json(new
            {
                Rows = userInfoList.Select(u => new
                {
                    u.ID,
                    u.UserName,
                    u.RealName,
                    RoleType = u.RoleType == 10 ? "管理员" : "普通员工",
                    CreateTime = u.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    LastLoginTime = u.LastLoginTime == DateTime.MinValue ? "从未登陆" : u.LastLoginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                }),
                AllCount = allCount
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
