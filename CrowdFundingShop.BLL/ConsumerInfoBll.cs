using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.BLL
{
    public class ConsumerInfoBll
    {
        public static Model.ConsumerInfo GetByID(long id)
        {
            return DAL.ConsumerInfoDal.GetByID(id);
        }
        public static Model.ConsumerInfo GetByWeiXinAccount(string id)
        {
            return DAL.ConsumerInfoDal.GetByWeiXinAccount(id);
        }
        public static bool Update(Model.ConsumerInfo entity)
        {
            return DAL.ConsumerInfoDal.Update(entity);
        }
        public static Model.OrderInfo GetByNumber(int number,long huodongid)
        {
            return DAL.ConsumerInfoDal.GetByNumber(number,huodongid);
        }
        public static string Add(Model.ConsumerInfo entity)
        {
            return DAL.ConsumerInfoDal.Add(entity);
        }
    }
}
