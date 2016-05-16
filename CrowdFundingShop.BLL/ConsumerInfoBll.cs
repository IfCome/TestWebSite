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
        public static Model.OrderInfo GetByNumber(int number, long huodongid)
        {
            return DAL.ConsumerInfoDal.GetByNumber(number, huodongid);
        }
        public static string Add(Model.ConsumerInfo entity)
        {
            return DAL.ConsumerInfoDal.Add(entity);
        }

        public static List<Model.ConsumerInfo> GetAllConsumerInfoList(int pageSize, int currentPage, string keyWords, out int allCount)
        {
            allCount = 0;
            List<Model.ConsumerInfo> consumerInfoList = DAL.ConsumerInfoDal.GetPageListByCondition(pageSize, currentPage, out allCount, keyWords);
            if (consumerInfoList == null)
            {
                consumerInfoList = new List<Model.ConsumerInfo>();
            }
            return consumerInfoList;
        }

        public static List<Model.ConsumerInfo> GetTop10ConsumerInfos()
        {
            int allCount = 0;
            List<Model.ConsumerInfo> consumerInfoList = DAL.ConsumerInfoDal.GetPageListByCondition(10, 1, out allCount);
            if (consumerInfoList == null)
            {
                consumerInfoList = new List<Model.ConsumerInfo>();
            }
            return consumerInfoList;
        }
    }
}
