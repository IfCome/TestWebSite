using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrowdFundingShop.DAL;
using CrowdFundingShop.Utility;
using CrowdFundingShop.Model;

namespace CrowdFundingShop.BLL
{
    public static class BackgroundUserBll
    {

        /// <summary>
        /// 登陆检测
        /// </summary>
        public static Model.BackgroundUserInfo Login(string UseName, string password, string ipAddress)
        {
            password = Security.getMD5ByStr(password);
            Model.BackgroundUserInfo userInfo = DAL.BackgroundUserInfoDal.GetInfoByUserNameAndPwd(UseName, password);
            if (userInfo != null)
            {
                Identity.LoginUserInfo = userInfo;
                // 登录成功，记录日志
                string logTitle = "登录系统";
                string logMsg = "登录系统";
                BLL.BackgroundUserBll_log.AddLog(logTitle, logMsg, ipAddress);

                // 修改最近登录时间
                DAL.BackgroundUserInfoDal.UpdateLastLoginTimeByID(DateTime.Now, userInfo.ID);
            }

            return userInfo;
        }

        public static Model.BackgroundUserInfo GetLoginUserInfo(long userId)
        {
            return DAL.BackgroundUserInfoDal.GetUserInfoByID(userId);
        }

        /// <summary>
        /// 判断用户名是否存在
        /// </summary>
        public static bool IsExistUserName(string userName)
        {
            return DAL.BackgroundUserInfoDal.GetCountByUserName(userName) > 0;
        }

        public static bool AddUserInfo(BackgroundUserInfo userInfo)
        {
            return DAL.BackgroundUserInfoDal.Add(userInfo);
        }

        public static List<Model.BackgroundUserInfo> GetAllUserInfoList(int pageSize, int currentPage, int roleType, string keyWords, out int allCount)
        {
            allCount = 0;
            List<Model.BackgroundUserInfo> userInfoList = DAL.BackgroundUserInfoDal.GetPageListByCondition(pageSize, currentPage, roleType, out allCount, keyWords);
            if (userInfoList == null)
            {
                userInfoList = new List<BackgroundUserInfo>();
            }
            return userInfoList;
        }

        public static Model.BackgroundUserInfo GetSingleUserInfo(long id)
        {
            return DAL.BackgroundUserInfoDal.GetUserInfoByID(id);
        }

        public static bool UpdateUserInfo(BackgroundUserInfo userInfo)
        {
            return DAL.BackgroundUserInfoDal.UpdateByID(userInfo);
        }

        /// <summary>
        /// 比较新旧用户信息，返回修改的字段内容
        /// </summary>
        public static string GetDiffContent(BackgroundUserInfo newUserInfo, BackgroundUserInfo oldUserInfo)
        {
            StringBuilder diffContent = new StringBuilder("");
            // 逐一比较
            if (newUserInfo.RoleType != oldUserInfo.RoleType)
            {
                diffContent.Append(string.Format("角色由\"{0}\"改为\"{1}\";", (oldUserInfo.RoleType == 10 ? "管理员" : "普通员工"), (newUserInfo.RoleType == 10 ? "管理员" : "普通员工")));
            }
            if (newUserInfo.RealName != oldUserInfo.RealName)
            {
                diffContent.Append(string.Format("姓名由\"{0}\"改为\"{1}\";", oldUserInfo.RealName, newUserInfo.RealName));
            }
            if (newUserInfo.Phone != oldUserInfo.Phone)
            {
                diffContent.Append(string.Format("手机由\"{0}\"改为\"{1}\";", oldUserInfo.Phone, newUserInfo.Phone));
            }
            if (newUserInfo.Email != oldUserInfo.Email)
            {
                diffContent.Append(string.Format("电子邮箱由\"{0}\"改为\"{1}\";", oldUserInfo.Email, newUserInfo.Email));
            }
            if (newUserInfo.QQ != oldUserInfo.QQ)
            {
                diffContent.Append(string.Format("QQ由\"{0}\"改为\"{1}\";", oldUserInfo.QQ, newUserInfo.QQ));
            }
            if (newUserInfo.HeadIcon != oldUserInfo.HeadIcon)
            {
                diffContent.Append("修改头像");
            }
            return diffContent.ToString();
        }
    }
}
