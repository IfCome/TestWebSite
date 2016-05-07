using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.BLL
{
    public class ShoppingCartBll
    {
        public static bool Add(Model.ShoppingCart entity)
        {
            return DAL.ShoppingCartDal.Add(entity);
        }
        public static bool Delete(Model.ShoppingCart entity)
        {
            return DAL.ShoppingCartDal.Delete(entity);
        }
        public static bool DeleteByHuoDongID(Model.ShoppingCart entity)
        {
            return DAL.ShoppingCartDal.DeleteByHuoDongID(entity);
        }
        public static List<Model.ShoppingCart> GetShoppingInfoByID(long id)
        {
            return DAL.ShoppingCartDal.GetShoppingInfoByID(id);
        }
        public static List<Model.ShoppingCart> GetShoppingInfoBySID(string sids)
        {
            return DAL.ShoppingCartDal.GetShoppingInfoBySID(sids);
        }
        public static bool IsExistShoppingCartByHuoDongID(long huodongid, long consumerID)
        {
            return DAL.ShoppingCartDal.IsExistShoppingCartByHuoDongID(huodongid, consumerID);
        }
        public static bool UpdateStoreCount(Model.ShoppingCart entity)
        {
            return DAL.ShoppingCartDal.UpdateStoreCount(entity);
        }
    }
}
