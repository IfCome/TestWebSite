using System;
using System.Collections.Generic;
using System.Data;
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

        public static DataTable GetTop10SimpleInfo()
        {
            return DAL.HuoDongInfoDal.GetTop10SimpleInfo();
        }

        public static Model.HuoDongInfo GetLuckNumberByID(long huodongid)
        {
            return DAL.HuoDongInfoDal.GetLuckNumberByID(huodongid);
        }

        public static bool Update(Model.HuoDongInfo entity)
        {
            return DAL.HuoDongInfoDal.Update(entity);
        }

        public static bool UpdateState(long huodongid,int huodongnumber)
        {
            return DAL.HuoDongInfoDal.UpdateState(huodongid, huodongnumber);
        }
        public static int GetMaxHuoDongNumByGoodsID(long goodsid)
        {
            return DAL.HuoDongInfoDal.GetMaxHuoDongNumByGoodsID(goodsid);
        }
    }
}
