using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.BLL
{
    public class OrderInfoBll
    {
        public static bool Add(Model.OrderInfo entity)
        {
            return DAL.OrderInfoDal.Add(entity);
        }
    }
}
