using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrowdFundingShop.Utility;

namespace CrowdFundingShop.BLL
{
    public static class BackgroundUserBll_log
    {
        public static bool AddLog(string title, string msg, string ipAddress)
        {
            Model.BackgroundUserInfo_log logEntity = new Model.BackgroundUserInfo_log()
            {
                UserID = Identity.LoginUserInfo.ID,
                OperateTile = title,
                OperateDetail = msg,
                OperateTime = DateTime.Now,
                IpAddress = ipAddress
            };

            return DAL.BackgroundUserInfoDal_log.Add(logEntity);
        }
    }
}
