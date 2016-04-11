using CrowdFundingShop.Utility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Data;

namespace CrowdFundingShop.DAL
{
    public class HuoDongInfoDal
    {
        #region 增加
        public static bool Add(Model.HuoDongInfo entity)
        {
            var sql = @"
                        INSERT INTO [HuoDongInfo]
                               (
                                    GoodsID
                                    ,ShareCount
                                    ,State
                                    ,CreateTime
                                    ,CreateUser
                                    ,FinishedTime	
                                    ,HuodongNumber
                                    ,LuckDogID
                                    ,LuckNumber
                                    ,IsDelete
                               )
                         VALUES
                               (
                                    @GoodsID
                                    ,@ShareCount
                                    ,@State
                                    ,GETDATE()
                                    ,@CreateUser
                                    ,@FinishedTime	
                                    ,@HuodongNumber
                                    ,@LuckDogID
                                    ,@LuckNumber
                                    ,0
                               )";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@GoodsID", Value = entity.GoodsID });
            parameters.Add(new SqlParameter() { ParameterName = "@ShareCount", Value = entity.ShareCount });
            parameters.Add(new SqlParameter() { ParameterName = "@State", Value = entity.State });
            parameters.Add(new SqlParameter() { ParameterName = "@CreateUser", Value = entity.CreateUser });
            parameters.Add(new SqlParameter() { ParameterName = "@FinishedTime", Value = entity.FinishedTime });
            parameters.Add(new SqlParameter() { ParameterName = "@HuodongNumber", Value = 0 });
            parameters.Add(new SqlParameter() { ParameterName = "@LuckDogID", Value = 0 });
            parameters.Add(new SqlParameter() { ParameterName = "@LuckNumber", Value = 0 });
            try
            {
                return SqlHelper.ExecuteNonQuery(sql, parameters.ToArray()) > 0;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        public static DataTable GetTop10SimpleInfo()
        {
            var sql = @"
                        WITH a 
                                AS ( 
                                --所有的众筹中的活动  
                                SELECT id, 
                                        goodsid, 
                                        sharecount,
                                        HuodongNumber 
                                    FROM   huodonginfo 
                                    WHERE  state = 10
                                           AND IsDelete=0
                                           AND (SELECT COUNT(ID) FROM GoodsBaseInfo WHERE ID=GoodsID AND IsDelete=0) > 0), 
                                b 
                                AS ( 
                                --所有的众筹中的活动的订单数 
                                SELECT huodongid, 
                                        Count(id) AS OrderCount 
                                    FROM   orderinfo 
                                    WHERE  huodongid IN (SELECT id 
                                                        FROM   a) 
                                    GROUP  BY huodongid), 
                                c 
                                AS ( 
                                --前十名活动 
                                SELECT TOP 10 a.*, 
                                                OrderCount = Isnull(b.ordercount, 0) 
                                    FROM   a 
                                        FULL JOIN b 
                                                ON a.id = b.huodongid) 
                                --整理所有信息 
                                SELECT  GoodsID=c.goodsid, 
                                        GoodsName = (SELECT TOP 1 goodsname 
                                                    FROM   goodsbaseinfo 
                                                    WHERE  id = c.goodsid), 
                                        Describe=(SELECT TOP 1 Describe
                                                            FROM   goodsbaseinfo 
                                                            WHERE  id = c.goodsid), 
                                        HuodongNumber,
                                        LastestCustomer = (SELECT TOP 1 NAME = (SELECT nickname 
                                                                                FROM   consumerinfo 
                                                                                WHERE  id = consumerid) 
                                                            FROM   orderinfo 
                                                            WHERE  huodongid = c.id 
                                                            ORDER  BY createtime DESC), 
                                        ShareCount, 
                                        OrderCount, 
                                        Progress = ordercount * 100.0 / c.sharecount, 
                                        DailyIncrease =(SELECT Count(id) 
                                                        FROM   orderinfo 
                                                        WHERE  huodongid = c.id) 
                                FROM   c 
                        ";
            var dataTable = SqlHelper.ExecuteDataTable(sql, null);

            return dataTable;
        }
    }
}
