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
                UserID = Identity.LoginUserInfo == null ? 0 : Identity.LoginUserInfo.ID,
                OperateTile = title,
                OperateDetail = msg,
                OperateTime = DateTime.Now,
                IpAddress = ipAddress
            };

            return DAL.BackgroundUserInfoDal_log.Add(logEntity);
        }

        public static List<Model.BackgroundUserInfo_log> GetSingleUserTop10Logs(long id)
        {
            return DAL.BackgroundUserInfoDal_log.GetTop10ListByUserID(id);
        }

        public static List<Model.BackgroundUserInfo_log> GetTop10Logs()
        {
            return DAL.BackgroundUserInfoDal_log.GetTop10List();
        }
    }
}
