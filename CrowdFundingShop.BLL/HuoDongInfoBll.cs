using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.BLL
{
    public class HuoDongInfoBll
    {
        public static bool Add(Model.HuoDongInfo entity)
        {
            return DAL.HuoDongInfoDal.Add(entity);
        }
    }
}
