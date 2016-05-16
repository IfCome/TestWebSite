using CrowdFundingShop.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.DAL
{
    public class ConsumerInfoDal
    {
        #region 增加
        public static string Add(Model.ConsumerInfo entity)
        {
            string sql = @"INSERT INTO [ConsumerInfo]
                                    (
                                    WeiXinAccount 
                                    ,NickName
                                    ,HeadIcon
                                    ,Address
                                    ,Phone
                                    )
                                VALUES
                                    (
                                    @WeiXinAccount 
                                    ,@NickName
                                    ,@HeadIcon
                                    ,@Address
                                    ,@Phone
                                    );SELECT SCOPE_IDENTITY()";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@WeiXinAccount", Value = entity.WeiXinAccount });
            parameters.Add(new SqlParameter() { ParameterName = "@NickName", Value = entity.NickName });
            parameters.Add(new SqlParameter() { ParameterName = "@HeadIcon", Value = entity.HeadIcon });
            parameters.Add(new SqlParameter() { ParameterName = "@Address", Value = entity.Address });
            parameters.Add(new SqlParameter() { ParameterName = "@Phone", Value = "" });
            try
            {
                return SqlHelper.ExecuteScalar(sql, parameters.ToArray()).ToString();
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region 修改
        public static bool Update(Model.ConsumerInfo entity)
        {
            string sql = @"UPDATE ConsumerInfo SET NickName=@NickName,Phone=@Phone,[Address]=@Address,PostCode=@PostCode WHERE ID=@ID";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@ID", Value = entity.ID });
            parameters.Add(new SqlParameter() { ParameterName = "@NickName", Value = entity.NickName });
            parameters.Add(new SqlParameter() { ParameterName = "@Phone", Value = entity.Phone });
            parameters.Add(new SqlParameter() { ParameterName = "@Address", Value = entity.Address });
            parameters.Add(new SqlParameter() { ParameterName = "@PostCode", Value = entity.PostCode });
            int i = SqlHelper.ExecuteNonQuery(sql, parameters.ToArray());
            return i > 0 ? true : false;
        }
        #endregion

        #region 查找
        public static Model.ConsumerInfo GetByID(long id)
        {
            var sql = @"
                              SELECT 
                                    ID
                                    ,WeiXinAccount
                                    ,NickName
                                    ,Phone
                                    ,HeadIcon
                                    ,[Address]
                                    ,PostCode
                                FROM ConsumerInfo 
                                WHERE ID=@ID";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@ID", Value = id });
            sql = string.Format(sql);
            DataTable dataTable = SqlHelper.ExecuteDataTable(sql, parameters.ToArray());
            if (dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];
                return new Model.ConsumerInfo()
                {
                    ID = Converter.TryToInt32(row["ID"], -1),
                    WeiXinAccount = Converter.TryToString(row["WeiXinAccount"], string.Empty),
                    NickName = Converter.TryToString(row["Nickname"], string.Empty),
                    Phone = Converter.TryToString(row["Phone"], string.Empty),
                    Address = Converter.TryToString(row["Address"], string.Empty),
                    PostCode = Converter.TryToString(row["PostCode"], string.Empty),
                };
            }
            return null;
        }

        public static Model.ConsumerInfo GetByWeiXinAccount(string WeiXinAccount)
        {
            var sql = @"
                              SELECT 
                                    ID
                                    ,WeiXinAccount
                                    ,NickName
                                    ,Phone
                                    ,HeadIcon
                                    ,[Address]
                                    ,PostCode
                                FROM ConsumerInfo 
                                WHERE WeiXinAccount=@WeiXinAccount";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@WeiXinAccount", Value = WeiXinAccount });
            sql = string.Format(sql);
            DataTable dataTable = SqlHelper.ExecuteDataTable(sql, parameters.ToArray());
            if (dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];
                return new Model.ConsumerInfo()
                {
                    ID = Converter.TryToInt32(row["ID"], -1),
                    WeiXinAccount = Converter.TryToString(row["WeiXinAccount"], string.Empty),
                    NickName = Converter.TryToString(row["Nickname"], string.Empty),
                    Phone = Converter.TryToString(row["Phone"], string.Empty),
                    Address = Converter.TryToString(row["Address"], string.Empty),
                    PostCode = Converter.TryToString(row["PostCode"], string.Empty),
                };
            }
            return null;
        }

        public static Model.OrderInfo GetByNumber(int number, long huodongid)
        {
            var sql = @"SELECT 
                                 ConsumerID
                               FROM OrderInfo
                               WHERE HuodongID=@HuodongID AND Number=@Number";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@HuodongID", Value = huodongid });
            parameters.Add(new SqlParameter() { ParameterName = "@Number", Value = number });
            sql = string.Format(sql);
            DataTable dataTable = SqlHelper.ExecuteDataTable(sql, parameters.ToArray());
            if (dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];
                return new Model.OrderInfo()
                {
                    ConsumerID = Converter.TryToInt64(row["ConsumerID"], 0)
                };
            }
            return null;
        }


        /// <summary>
        /// 根据指定条件分页查询客户数据
        /// </summary>
        /// <param name="pageSize">每页数据量</param>
        /// <param name="currentPage">页码</param>
        /// <param name="keyWords">搜索关键字</param>
        /// <param name="allCount">用户总量</param>
        public static List<Model.ConsumerInfo> GetPageListByCondition(int pageSize, int currentPage, out int allCount, string keyWords = "")
        {
            var sql = @"
                        WITH    A AS ( SELECT   COUNT(1) AS 'JiJiangJieXiao' ,
                                                ConsumerID
                                       FROM     OrderInfo O
                                                JOIN HuodongInfo H ON O.HuodongID = H.ID
                                       WHERE    H.State = 10
                                       GROUP BY ConsumerID
                                     ),
                                B AS ( SELECT   COUNT(1) AS 'YiJieXiao' ,
                                                ConsumerID
                                       FROM     OrderInfo O
                                                JOIN HuodongInfo H ON O.HuodongID = H.ID
                                       WHERE    H.State = 30
                                       GROUP BY ConsumerID
                                     ),
                                C AS ( SELECT   SUM(StoreCount) AS 'CartCount' ,
                                                ConsumerID
                                       FROM     ShoppingCart S
                                                JOIN HuodongInfo H ON S.HuodongID = H.ID
                                       WHERE    STATE = 10
                                       GROUP BY ConsumerID
                                     ),
                                D AS ( SELECT   ID ,
                                                WeiXinAccount ,
                                                Nickname ,
                                                Phone ,
                                                HeadIcon ,
                                                [Address]
                                       FROM     ConsumerInfo
                                       {0}
                                     ),
                                result
                                  AS ( SELECT   D.* ,
                                                JiJiangJieXiao = ISNULL(A.JiJiangJieXiao, 0) ,
                                                YiJieXiao = ISNULL(B.YiJieXiao, 0) ,
                                                CartCount = ISNULL(C.CartCount, 0) ,
                                                ROW_NUMBER() OVER ( ORDER BY ID DESC ) AS [RowNumber]
                                       FROM     D
                                                LEFT	JOIN A ON D.ID = A.ConsumerID
                                                LEFT	JOIN B ON D.ID = B.ConsumerID
                                                LEFT	JOIN C ON D.ID = C.ConsumerID
                                     )
                            SELECT  *
                            FROM    result 
                            WHERE   @PageSize * ( @CurrentPage - 1 ) < RowNumber
                                    AND RowNumber <= @PageSize * @CurrentPage
                    ";

            //条件查询部分
            var sqlWhere = "";
            var parameters = new List<SqlParameter>();
            if (keyWords.IsNullOrWhiteSpace() == false)
            {
                sqlWhere += "  WHERE (Nickname LIKE @KeyWord OR Address LIKE @KeyWord OR Phone LIKE @KeyWord ) ";
                parameters.Add(new SqlParameter() { ParameterName = "@KeyWord", Value = "%" + keyWords + "%" });
            }

            sql = string.Format(sql, sqlWhere);
            parameters.Add(new SqlParameter() { ParameterName = "@PageSize", Value = pageSize });
            parameters.Add(new SqlParameter() { ParameterName = "@CurrentPage", Value = currentPage });

            //记录总数计算
            var countParameters = new List<SqlParameter>();
            parameters.ForEach(h => countParameters.Add(new SqlParameter() { ParameterName = h.ParameterName, Value = h.Value }));
            var sqlCount = string.Format("SELECT COUNT(*) CNT FROM [ConsumerInfo] {0} ", sqlWhere);
            allCount = Converter.TryToInt32(SqlHelper.ExecuteScalar(sqlCount, countParameters.ToArray()));

            if (allCount == 0)
            {
                return null;
            }

            var dataTable = SqlHelper.ExecuteDataTable(sql, parameters.ToArray());
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.AsEnumerable().Select(row => new Model.ConsumerInfo()
                {
                    ID = Converter.TryToInt64(row["ID"], -1),
                    WeiXinAccount = Converter.TryToString(row["WeiXinAccount"], string.Empty),
                    NickName = Converter.TryToString(row["Nickname"], string.Empty),
                    Phone = Converter.TryToString(row["Phone"], string.Empty),
                    HeadIcon = Converter.TryToString(row["HeadIcon"], string.Empty),
                    JiJiangJieXiao = Converter.TryToInt32(row["JiJiangJieXiao"], 0),
                    YiJieXiao = Converter.TryToInt32(row["YiJieXiao"], 0),
                    CartCount = Converter.TryToInt32(row["CartCount"], 0),
                    Address = Converter.TryToString(row["Address"], string.Empty)
                }).ToList();
            }
            else
            {
                return null;
            }
        }

        #endregion

    }
}
