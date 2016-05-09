using CrowdFundingShop.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.DAL
{
    public class OrderInfoDal
    {
        #region 增
        public static bool Add(Model.OrderInfo entity)
        {
            var sql = @"
                        INSERT INTO [OrderInfo]
                               (
                                    ConsumerID
                                    ,HuodongID
                                    ,Number
                                    ,CreateTime
                               )
                         VALUES
                               (
                                    @ConsumerID
                                    ,@HuodongID
                                    ,@Number
                                    ,GETDATE()
                               )";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@ConsumerID", Value = entity.ConsumerID });
            parameters.Add(new SqlParameter() { ParameterName = "@HuodongID", Value = entity.HuodongID });
            parameters.Add(new SqlParameter() { ParameterName = "@Number", Value = entity.Number });
            try
            {
                return SqlHelper.ExecuteNonQuery(sql, parameters.ToArray()) > 0;
            }
            catch
            {
                return false;
            }
        }

        public static bool AddRandom(string randomnum, long huodongid)
        {
            var sql = @"
                        INSERT INTO [RandomInfo]
                               (
                                    RandomNum
                                    ,HuodongID
                               )
                         VALUES
                               (
                                    @RandomNum
                                    ,@HuodongID    
                               )";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@RandomNum", Value = randomnum });
            parameters.Add(new SqlParameter() { ParameterName = "@HuodongID", Value = huodongid });
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
        #region 删
        #endregion
        #region 改
        #endregion
        #region 查
        public static List<Model.GoodsBaseInfo> GetList(int type, string huodongstate, long consumerid, int isMine)
        {
            string sql = @"SELECT
                                        G.ID
                                        ,G.GoodsName
                                        ,G.Describe
                                        ,G.Price
                                        ,G.Category
                                        ,C.CategoryName
                                        ,G.ShowIcons
                                        ,G.DetailIcons
                                        ,G.CreateTime
                                        ,G.CreateUserID
                                        ,State=CASE WHEN State IS NULL THEN '0'
                                        ELSE State END
                                        ,ZhongChouCount=(SELECT COUNT(1) FROM OrderInfo WHERE HuoDongID=H.ID)
                                        ,FinishedTime
                                        ,H.ID AS HuoDongID
                                        ,S.NickName
                                        ,H.LuckNumber
                                        ,H.HuoDongNumber
                                    FROM GoodsBaseInfo G WITH (NOLOCK) JOIN CategoryInfo C WITH (NOLOCK)
                                    ON G.Category=C.ID LEFT JOIN dbo.HuodongInfo H
                                    ON G.ID=H.GoodsID LEFT JOIN ConsumerInfo S
                                    ON H.LuckDogID=S.ID
                                    WHERE G.IsDelete=0 AND C.IsDelete=0 AND H.ID IN (SELECT HuodongID FROM OrderInfo WHERE ConsumerID=@ConsumerID) {0}";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@ConsumerID", Value = consumerid });
            string sqlWhere1 = string.Empty;
            //更改逻辑  所有正在众筹的商品都是即将揭晓
            //if (type == 1)
            //{
            //    sqlWhere2 += " Where ZhongChouCount>=(0.6*Price)";
            //}
            if (!string.IsNullOrEmpty(huodongstate))
            {
                sqlWhere1 += " AND State=@State";
                parameters.Add(new SqlParameter() { ParameterName = "@State", Value = huodongstate });
            }
            if (isMine == 1)
            {
                sqlWhere1 += " AND LuckDogID=@ConsumerID";
            }
            sql = string.Format(sql, sqlWhere1);
            DataTable dataTable = SqlHelper.ExecuteDataTable(sql, parameters.ToArray());
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.AsEnumerable().Select(row => new Model.GoodsBaseInfo()
                {
                    ID = Converter.TryToInt32(row["ID"], -1),
                    GoodsName = Converter.TryToString(row["GoodsName"], string.Empty),
                    Category = Converter.TryToString(row["CategoryName"], string.Empty),
                    CreateUserID = Converter.TryToString(row["CreateUserID"], string.Empty),
                    Price = Converter.TryToString(row["Price"], string.Empty),
                    Describe = Converter.TryToString(row["Describe"], string.Empty),
                    DetailIcons = Converter.TryToString(row["DetailIcons"], string.Empty),
                    ShowIcons = Converter.TryToString(row["ShowIcons"], string.Empty),
                    CreateTime = Converter.TryToString(row["CreateTime"], DateTime.MinValue.ToString()),
                    State = Converter.TryToInt32(row["State"], -1),
                    ZhongChouCount = Converter.TryToInt32(row["ZhongChouCount"], 0),
                    FinishedTime = Converter.TryToDateTime(row["FinishedTime"], DateTime.MinValue),
                    HuoDongID = Converter.TryToInt64(row["HuoDongID"], -1),
                    LuckNickName = Converter.TryToString(row["NickName"], string.Empty),
                    LuckNumber = Converter.TryToInt64(row["LuckNumber"], -1),
                    HuoDongNumber = Converter.TryToInt32(row["HuoDongNumber"], -1),
                }).ToList();
            }
            return null;
        }
        public static List<Model.OrderInfo> GetOrderListByHuoDongID(long huodongid, long consumerid)
        {
            string sql = @"SELECT ID,HuoDongID,Number FROM OrderInfo WHERE HuoDongID=@HuoDongID AND ConsumerID=@ConsumerID";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@HuoDongID", Value = huodongid });
            parameters.Add(new SqlParameter() { ParameterName = "@ConsumerID", Value = consumerid });
            DataTable dataTable = SqlHelper.ExecuteDataTable(sql, parameters.ToArray());
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.AsEnumerable().Select(row => new Model.OrderInfo()
                {
                    ID = Converter.TryToInt32(row["ID"], -1),
                    HuodongID = Converter.TryToInt64(row["HuoDongID"], 0),
                    Number = Converter.TryToInt32(row["Number"], 0)
                }).ToList();
            }
            return null;
        }
        //查当前已有号码最大的
        public static int GetMaxNumber(long huodongid, long consumerid)
        {
            string sql = @"SELECT MAX(Number) from OrderInfo WHERE HuodongID=@HuoDongID AND ConsumerID=@ConsumerID";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@HuoDongID", Value = huodongid });
            parameters.Add(new SqlParameter() { ParameterName = "@ConsumerID", Value = consumerid });
            DataTable dataTable = SqlHelper.ExecuteDataTable(sql, parameters.ToArray());
            if (dataTable.Rows.Count > 0)
            {
                return Converter.TryToInt32(dataTable.Rows[0][0]) > 0 ? Converter.TryToInt32(dataTable.Rows[0][0]) : 1000000;
            }
            return 1000000;
        }
        public static Model.ConsumerInfo GetKeyCount(long consumerid)
        {
            string sql = @"WITH A AS
                                    (
                                                    SELECT COUNT(1) AS 'JiJiangJieXiao' FROM OrderInfo O join HuodongInfo H
                                                    ON O.HuodongID=H.ID
                                        WHERE H.State=10 AND ConsumerID=@ConsumerID
                                    )
                                    , B AS
                                    (
                                       SELECT COUNT(1) AS 'YiJieXiao' FROM OrderInfo O join HuodongInfo H
                                                    ON O.HuodongID=H.ID
                                        WHERE H.State=30 AND ConsumerID=@ConsumerID
                                    )
                                    ,C AS
                                    (
                                                    SELECT SUM(StoreCount) AS 'CartCount' FROM ShoppingCart S JOIN HuodongInfo H
                                                    ON S.HuodongID=H.ID
                                                    WHERE ConsumerID=@ConsumerID AND STATE=10
                                    )
                                    ,D AS
                                    (
                                                    SELECT ID,WeiXinAccount,Nickname,Phone,HeadIcon FROM ConsumerInfo WHERE ID=@ConsumerID
                                    )
                                    SELECT * FROM A,B,C,D";
            List<SqlParameter> parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter() { ParameterName = "@ConsumerID", Value = consumerid });
            DataTable dataTable = new DataTable();
            dataTable = SqlHelper.ExecuteDataTable(sql, parameter.ToArray());
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];
                return new Model.ConsumerInfo()
                {
                    ID = Converter.TryToInt32(row["ID"], 0),
                    WeiXinAccount = Converter.TryToString(row["WeiXinAccount"], ""),
                    HeadIcon = Converter.TryToString(row["HeadIcon"], ""),
                    NickName = Converter.TryToString(row["Nickname"], ""),
                    Phone = Converter.TryToString(row["Phone"], ""),
                    JiJiangJieXiao = Converter.TryToInt32(row["JiJiangJieXiao"], 0),
                    YiJieXiao = Converter.TryToInt32(row["YiJieXiao"], 0),
                    CartCount = Converter.TryToInt32(row["CartCount"], 0),
                };
            }
            return null;
        }
        //查这个活动下的获奖信息
        public static List<Model.OrderInfo> GetDrawnPrizeUser(long huodongid)
        {
            string sql = @"SELECT
                                     C.ID AS ConsumerID
                                     Nickname,
                                     Max(CreateTime) AS 'CreateTime',
                                     COUNT(1) AS 'StoreCount'
                                 FROM ORDERINFO O JOIN CONSUMERINFO C
                                 ON O.ConsumerID=C.ID
                                 WHERE HuodongID=@HuoDongID
                                 GROUP BY Nickname,HuodongID
                                 ORDER BY StoreCount DESC";
            List<SqlParameter> parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter() { ParameterName = "@HuoDongID", Value = huodongid });
            DataTable dataTable = new DataTable();
            dataTable = SqlHelper.ExecuteDataTable(sql, parameter.ToArray());
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return dataTable.AsEnumerable().Select(row => new Model.OrderInfo()
                {
                    ID = Converter.TryToInt32(row["ConsumerID"], 0),
                    NickName = Converter.TryToString(row["NickName"], ""),
                    CreateTime = Converter.TryToDateTime(row["CreateTime"], DateTime.MinValue),
                    StoreCount = Converter.TryToInt32(row["StoreCount"], 0),
                }).ToList();
            }
            return null;
        }


        //查某个人的信息
        public static List<Model.OrderInfo> GetNumberByHuoDongAndUser(long huodongid, long consumerid)
        {
            string sql = @"SELECT
                                      Number
                                   FROM OrderInfo
                                   WHERE HuodongID=@HuodongID AND ConsumerID=@ConsumerID";
            List<SqlParameter> parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter() { ParameterName = "@HuoDongID", Value = huodongid });
            parameter.Add(new SqlParameter() { ParameterName = "@ConsumerID", Value = consumerid });
            DataTable dataTable = new DataTable();
            dataTable = SqlHelper.ExecuteDataTable(sql, parameter.ToArray());
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return dataTable.AsEnumerable().Select(row => new Model.OrderInfo()
                {
                    Number = Converter.TryToInt32(row["Number"], 0),
                }).ToList();
            }
            return null;
        }

        public static List<DateTime> GetTop10OrderTimeList(long huodongid)
        {
            string sql = @"
                            SELECT
	                            CreateTime
                            FROM OrderInfo
                            WHERE huodongid = @HuoDongID
                            ORDER BY CreateTime ASC";
            List<SqlParameter> parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter() { ParameterName = "@HuoDongID", Value = huodongid });
            DataTable dataTable = new DataTable();
            dataTable = SqlHelper.ExecuteDataTable(sql, parameter.ToArray());
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return dataTable.AsEnumerable().Select(row => Converter.TryToDateTime(row["CreateTime"], DateTime.MinValue)).ToList();
            }
            return null;
        }
        public static List<DateTime> GetLast10OrderTimeList(long huodongid)
        {
            string sql = @"
                            SELECT
	                            CreateTime
                            FROM OrderInfo
                            WHERE huodongid = @HuoDongID
                            ORDER BY CreateTime DESC";
            List<SqlParameter> parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter() { ParameterName = "@HuoDongID", Value = huodongid });
            DataTable dataTable = new DataTable();
            dataTable = SqlHelper.ExecuteDataTable(sql, parameter.ToArray());
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return dataTable.AsEnumerable().Select(row => Converter.TryToDateTime(row["CreateTime"], DateTime.MinValue)).ToList();
            }
            return null;
        }
        #endregion
    }
}
