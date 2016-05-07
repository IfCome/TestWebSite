using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrowdFundingShop.Utility;

namespace CrowdFundingShop.BLL
{
    public class OrderInfoBll
    {
        public static bool Add(Model.OrderInfo entity)
        {
            return DAL.OrderInfoDal.Add(entity);
        }
        public static bool AddRandom(string randomnum, long huodongid)
        {
            return DAL.OrderInfoDal.AddRandom(randomnum, huodongid);
        }
        public static List<Model.GoodsBaseInfo> GetList(int type, long consumerid, int isMine)
        {
            string huodongstate = "";
            if (type == 1)
            {
                huodongstate = "10";
            }
            else if (type == 2)
            {
                huodongstate = "30";
            }
            List<Model.GoodsBaseInfo> list = DAL.OrderInfoDal.GetList(type, huodongstate, consumerid, isMine);
            if (list != null)
            {
                string number = "";
                foreach (var item in list)
                {
                    List<Model.OrderInfo> listorder = DAL.OrderInfoDal.GetOrderListByHuoDongID(item.HuoDongID, consumerid);
                    if (listorder != null)
                    {
                        foreach (var orderinfo in listorder)
                        {
                            number += orderinfo.Number + ",";
                        }
                        number = string.IsNullOrEmpty(number) ? "" : number.Remove(number.Length - 1);
                    }
                    item.Number = number;
                    number = "";
                    item.StoreCount = listorder == null ? 0 : listorder.Count;
                }
            }
            return list;
        }
        public static int GetMaxNumber(long huodongid, long consumerid)
        {
            return DAL.OrderInfoDal.GetMaxNumber(huodongid, consumerid);
        }
        public static Model.ConsumerInfo GetKeyCount(long consumerid)
        {
            return DAL.OrderInfoDal.GetKeyCount(consumerid);
        }

        public static List<Model.OrderInfo> GetDrawnPrizeUser(long huodongid)
        {
            return DAL.OrderInfoDal.GetDrawnPrizeUser(huodongid);
        }

        public static long GetTop10OrderTimeSum(long huodongid, int allPrice)
        {
            Random rd = new Random();
            long result = (rd.Next(0, allPrice) + 1000001) * 10; // 默认随机取一个值*10
            List<DateTime> timeList = DAL.OrderInfoDal.GetTop10OrderTimeList(huodongid);
            if (timeList != null && timeList.Count > 0)
            {
                result = 0;
                foreach (DateTime item in timeList)
                {
                    result += Converter.TryToInt64(item.ToString("ddHHmmss"));
                }
            }
            return result;
        }


        public static long GetLast10OrderTimeSum(long huodongid, int allPrice)
        {
            Random rd = new Random();
            long result = (rd.Next(0, allPrice) + 1000001) * 10; // 默认随机取一个值*10
            List<DateTime> timeList = DAL.OrderInfoDal.GetLast10OrderTimeList(huodongid);
            if (timeList != null && timeList.Count > 0)
            {
                result = 0;
                foreach (DateTime item in timeList)
                {
                    result += Converter.TryToInt64(item.ToString("ddHHmmss"));
                }
            }
            return result;
        }
    }
}
