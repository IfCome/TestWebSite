using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.BLL
{
    public class GoodsBaseInfoBll
    {
        public static string AddGoodsInfo(Model.GoodsBaseInfo InModel)
        {
            return DAL.GoodsBaseInfoDal.AddGoodsInfo(InModel);
        }
        public static List<Model.GoodsBaseInfo> GetList(int currentPage, string keyWords, out int allCount)
        {
            return DAL.GoodsBaseInfoDal.GetList(10, currentPage, keyWords, out allCount);
        }
    }
}
