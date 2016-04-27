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
        public static List<Model.GoodsBaseInfo> GetList(int type, long consumerid,int isMine)
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
    }
}
