﻿using System;
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
        public static List<Model.GoodsBaseInfo> GetList(int pageSize,int currentPage, string keyWords,string huodongstate, out int allCount)
        {
            return DAL.GoodsBaseInfoDal.GetList(pageSize, currentPage, keyWords, huodongstate,out allCount);
        }
        public static bool UpDateGoodsInfo(Model.GoodsBaseInfo InModel)
        {
            return DAL.GoodsBaseInfoDal.UpDateGoodsInfo(InModel);
        }
        public static Model.GoodsBaseInfo GetGoodsInfoByID(string id)
        {
            return DAL.GoodsBaseInfoDal.GetGoodsInfoByID(id);
        }
        public static bool DeleteGoodsInfo(string id)
        {
            return DAL.GoodsBaseInfoDal.DeleteGoodsInfo(id);
        }
    }
}
