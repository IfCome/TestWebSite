using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.BLL
{
    public static class BackgroundUserBll_log
    {
        public static bool AddLog(Model.BackgroundUserInfo_log logEntity)
        {
            return DAL.BackgroundUserInfoDal_log.Add(logEntity);
        }
    }
}
