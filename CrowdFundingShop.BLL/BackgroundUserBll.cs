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
        public static  Model.BackgroundUserInfo Login(string UseName, string password)
        {
            password = Security.getMD5ByStr(password);
            return DAL.BackgroundUserInfoDal.GetInfoByUserNameAndPwd(UseName, password);
        }
    }
}
