using CrowdFundingShop.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.DAL
{
    public class ShoppingCartDal
    {
        #region 增
        public static bool Add(Model.ShoppingCart entity)
        {
            var sql = @"
                        INSERT INTO [ShoppingCart]
                               (
                                [ConsumerID]
                                ,[HuoDongID]
                                ,[StoreCount]
                               )
                         VALUES
                               (
                                @ConsumerID
                                ,@HuoDongID
                                ,@StoreCount
                               )
                    ";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@ConsumerID", Value = entity.ConsumerID });
            parameters.Add(new SqlParameter() { ParameterName = "@HuoDongID", Value = entity.HuoDongID });
            parameters.Add(new SqlParameter() { ParameterName = "@StoreCount", Value = entity.StoreCount });
            int i = SqlHelper.ExecuteNonQuery(sql, parameters.ToArray());
            return i > 0 ? true : false;
        }
        #endregion
        #region 删
        public static bool Delete(Model.ShoppingCart entity)
        {
            var sql = @"DELETE FROM ShoppingCart WHERE ID=@ID AND ConsumerID=@ConsumerID";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@ID", Value = entity.ID });
            parameters.Add(new SqlParameter() { ParameterName = "@ConsumerID", Value = entity.ConsumerID });
            int i = SqlHelper.ExecuteNonQuery(sql, parameters.ToArray());
            return i > 0 ? true : false;
        }
        #endregion
        public static bool UpdateStoreCount(Model.ShoppingCart entity)
        {
            var sql = string.Empty;
            var parameters = new List<SqlParameter>();
            if (entity.Type == 1)
            {
                sql = @"UPDATE ShoppingCart SET StoreCount=(StoreCount+@StoreCount) WHERE HuoDongID=@HuoDongID AND ConsumerID=@ConsumerID";
                parameters.Add(new SqlParameter() { ParameterName = "@HuoDongID", Value = entity.HuoDongID });
            }
            else if (entity.Type == 2) 
            {
                sql = @"UPDATE ShoppingCart SET StoreCount=@StoreCount WHERE ID=@ID AND ConsumerID=@ConsumerID";
                parameters.Add(new SqlParameter() { ParameterName = "@ID", Value = entity.ID });
            }
            parameters.Add(new SqlParameter() { ParameterName = "@StoreCount", Value = entity.StoreCount });
            parameters.Add(new SqlParameter() { ParameterName = "@ConsumerID", Value = entity.ConsumerID });
            int i = SqlHelper.ExecuteNonQuery(sql, parameters.ToArray());
            return i > 0 ? true : false;
        }
        #region 改
        #endregion
        #region 查
        //查自己购物车商品
        public static List<Model.ShoppingCart> GetShoppingInfoByID(long id)
        {
            var sql = @"SELECT 
                                    S.ID,
                                    HuodongID,
                                    StoreCount,
                                    GoodsID,
                                    GoodsName,
                                    Price,
                                    ZhongChouCount=(SELECT Count(1) FROM OrderInfo where huodongid=H.ID),
                                    ShowIcons
                                FROM ShoppingCart S JOIN HuoDongInfo H
                                    ON S.HuoDongID=H.ID JOIN GoodsBaseInfo G
                                    ON H.GoodsID=G.ID
                                WHERE ConsumerID=@ID";
            var parameters = new List<SqlParameter>();
            sql = string.Format(sql);
            parameters.Add(new SqlParameter() { ParameterName = "@ID", Value = id });
            DataTable dataTable = SqlHelper.ExecuteDataTable(sql, parameters.ToArray());
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.AsEnumerable().Select(row => new Model.ShoppingCart()
                 {
                     ID = Converter.TryToInt32(row["ID"], -1),
                     GoodsName = Converter.TryToString(row["GoodsName"], string.Empty),
                     Price = Converter.TryToString(row["Price"], string.Empty),
                     ShowIcons = Converter.TryToString(row["ShowIcons"], string.Empty),
                     ZhongChouCount = Converter.TryToInt32(row["ZhongChouCount"], 0),
                     HuoDongID = Converter.TryToInt64(row["HuoDongID"], 0),
                     StoreCount = Converter.TryToInt32(row["StoreCount"], 0),
                     GoodsID = Converter.TryToInt64(row["GoodsID"], 0),
                 }).ToList();
            }
            return null;
        }
        //根据主键ID查购物车信息
        public static List<Model.ShoppingCart> GetShoppingInfoBySID(string sids)
        {
            var sql = @"SELECT 
                                    S.ID,
                                    HuodongID,
                                    StoreCount,
                                    GoodsID,
                                    GoodsName,
                                    Price,
                                    ZhongChouCount=(SELECT Count(1) FROM OrderInfo where huodongid=H.ID),
                                    ShowIcons
                                FROM ShoppingCart S JOIN HuoDongInfo H
                                    ON S.HuoDongID=H.ID JOIN GoodsBaseInfo G
                                    ON H.GoodsID=G.ID
                                WHERE S.ID IN (" + sids + ")";
            DataTable dataTable = SqlHelper.ExecuteDataTable(sql);
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.AsEnumerable().Select(row => new Model.ShoppingCart()
                {
                    ID = Converter.TryToInt32(row["ID"], -1),
                    GoodsName = Converter.TryToString(row["GoodsName"], string.Empty),
                    Price = Converter.TryToString(row["Price"], string.Empty),
                    ShowIcons = Converter.TryToString(row["ShowIcons"], string.Empty),
                    ZhongChouCount = Converter.TryToInt32(row["ZhongChouCount"], 0),
                    HuoDongID = Converter.TryToInt64(row["HuoDongID"], 0),
                    StoreCount = Converter.TryToInt32(row["StoreCount"], 0),
                    GoodsID = Converter.TryToInt64(row["GoodsID"], 0),
                }).ToList();
            }
            return null;
        }
        //根据活动id判断是否已经存在这样商品
        public static bool IsExistShoppingCartByHuoDongID(long huodongid)
        {
            var sql = @"select count(1) from shoppingcart where huodongid=@HuoDongID";
            var parameters = new List<SqlParameter>();
            sql = string.Format(sql);
            parameters.Add(new SqlParameter() { ParameterName = "@HuoDongID", Value = huodongid });
            DataTable dataTable = SqlHelper.ExecuteDataTable(sql, parameters.ToArray());
            if (dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];
                return Converter.TryToInt32(row[0]) > 0;
            }
            return false;
        }
        #endregion
    }
}
