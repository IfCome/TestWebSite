using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.HtmlControls;
using CrowdFundingShop.Utility;

namespace CrowdFundingShop.UI.Controllers.PC
{
    public class UserInfoController : PCBaseController
    {
        //
        // GET: /UserInfo/

        // 用户管理页面
        public ActionResult IndexPage(string keyWords)
        {
            ViewBag.KeyWords = Converter.TryToString(keyWords, "");
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
                    string logTitle = "添加后台用户";
                    string logMsg = string.Format("添加用户信息：用户名【{0}】，角色【{1}】", userInfo.UserName, userInfo.RoleType == 10 ? "管理员" : "普通用户");
                    BLL.BackgroundUserBll_log.AddLog(logTitle, logMsg, Request.UserHostAddress);
                }
            }
            return Json(new { Message = msg, ErrorType = errorType });
        }


        // 编辑用户页面
        public ActionResult EditUser(long id)
        {
            if (Identity.LoginUserInfo.RoleType != 10)
            {
                return Content("只有管理员才能编辑用户");
            }
            // 查询用户信息
            Model.BackgroundUserInfo userInfo = BLL.BackgroundUserBll.GetSingleUserInfo(id);
            if (userInfo == null)
            {
                return Content("该用户不存在！！");
            }
            return View(userInfo);
        }


        [HttpPost]
        public ActionResult EditUserCallBack(Models.PC.BackGroundUserInfoModelIn InModel)
        {
            string errorType = "";
            string msg = "OK";
            if (Identity.LoginUserInfo.RoleType != 10)
            {
                errorType = "alert";
                msg = "只有管理员才能修改用户信息";
            }
            // 验证参数
            else if (InModel.ID == 0)
            {
                errorType = "alert";
                msg = "用户信息无效";
            }
            else
            {
                // 查询用户原信息
                Model.BackgroundUserInfo oldUserInfo = BLL.BackgroundUserBll.GetSingleUserInfo(InModel.ID);

                // 整理用户新信息
                Model.BackgroundUserInfo newUserInfo = new Model.BackgroundUserInfo()
                {
                    ID = InModel.ID,
                    RoleType = Converter.TryToInt32(InModel.RoleType, 20),
                    RealName = InModel.RealName ?? "",
                    Phone = InModel.Phone ?? "",
                    Email = InModel.Email ?? "",
                    QQ = InModel.QQ ?? "",
                    HeadIcon = InModel.HeadIcon ?? ""
                };

                if (BLL.BackgroundUserBll.UpdateUserInfo(newUserInfo) == false)
                {
                    errorType = "alert";
                    msg = "保存失败，请刷新用户列表重试";
                }
                else
                {
                    // 比较新旧用户信息，整理日志内容
                    string diffContent = BLL.BackgroundUserBll.GetDiffContent(newUserInfo, oldUserInfo);
                    // 记录日志
                    string logTitle = "修改后台用户";
                    string logMsg = string.Format("修改用户信息：用户名【{0}】，修改信息【{1}】", oldUserInfo.UserName, diffContent);
                    BLL.BackgroundUserBll_log.AddLog(logTitle, logMsg, Request.UserHostAddress);
                }
            }
            return Json(new { Message = msg, ErrorType = errorType });
        }


        [HttpGet]
        public ActionResult GetUserInfoList(Models.PC.GetUserInfoListIn InModel)
        {
            InModel.PageSize = Converter.TryToInt32(InModel.PageSize, 15);
            InModel.CurrentPage = Converter.TryToInt32(InModel.CurrentPage, 1);
            InModel.RoleType = Converter.TryToInt32(InModel.RoleType, -1);
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

        [HttpPost]
        public ActionResult UploadIcon()
        {
            if (Request.Files.Count == 0)
            {
                return Json(new { Message = "请选择文件！" });
            }
            HttpPostedFileBase imgFile = Request.Files[0];  // 获取上传文件
            if (imgFile.ContentLength == 0)
            {

                return Json(new { Message = "请选择文件！" });
            }
            #region 校验文件格式是否正确

            // 根据文件后缀名判断文件类型
            String extendname = imgFile.FileName.Substring(imgFile.FileName.LastIndexOf('.') + 1).ToLower();
            if (extendname != "jpg" && extendname != "jpeg" && extendname != "bmp" &&
                extendname != "gif" && extendname != "tiff" && extendname != "png")
            {
                return Json(new { Message = "文件类型错误！" });
            }

            #endregion

            // 上传图片到服务器
            FileUpToImg file = new FileUpToImg();

            String imgUrl = String.Empty;       // 图片在服务器端的保存地址
            imgUrl = file.uploadUrl;
            file.UpLoadImage(imgFile, "/images/", "s_", 150, 150);

            if (String.IsNullOrEmpty(imgUrl))
            {
                return Json(new { Message = "文件上传失败！" });
            }
            return Json(new { Message = "OK", ImgUrl = imgUrl });
        }
    }
}
