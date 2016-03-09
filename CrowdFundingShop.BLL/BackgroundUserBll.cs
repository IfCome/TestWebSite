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
        public static Model.BackgroundUserInfo Login(string UseName, string password)
        {
            password = Security.getMD5ByStr(password);
            return DAL.BackgroundUserInfoDal.GetInfoByUserNameAndPwd(UseName, password);
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
    }
}
