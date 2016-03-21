using CrowdFundingShop.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.DAL
{
    public class GoodsBaseInfoDal
    {
        #region 增加
        /// <summary>
        /// 增加后返回ID提供给下一个页面进行更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string AddGoodsInfo(Model.GoodsBaseInfo entity)
        {
            var sql = @"
                        INSERT INTO [GoodsBaseInfo]
                               (
                                GoodsName 
                                ,Price
                                ,Describe
                                ,Category
                                ,CreateUserID
                                ,CreateTime               
                                ,IsDelete
                               )
                         VALUES
                               (
                                @GoodsName 
                                ,@Price
                                ,@Describe
                                ,@Category
                                ,@CreateUserID
                                ,@CreateTime
                                ,0
                               );SELECT SCOPE_IDENTITY()";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@GoodsName", Value = entity.GoodsName });
            parameters.Add(new SqlParameter() { ParameterName = "@Price", Value = entity.Price });
            parameters.Add(new SqlParameter() { ParameterName = "@Describe", Value = entity.Describe });
            parameters.Add(new SqlParameter() { ParameterName = "@Category", Value = entity.Category });
            parameters.Add(new SqlParameter() { ParameterName = "@CreateUserID", Value = entity.CreateUserID });
            parameters.Add(new SqlParameter() { ParameterName = "@CreateTime", Value = entity.CreateTime });
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
        #region 查询
        public static List<Model.GoodsBaseInfo> GetList(int pageSize, int currentPage, string keyWords, out int allCount)
        {
            var sql = @"
                        WITH Virtual_T AS
                        (
	                           SELECT 
                                   G.ID
                                   ,GoodsName
                                   ,Describe
                                   ,Price
                                   ,Category
                                   ,CategoryName
                                   ,ShowIcons
                                   ,DetailIcons
                                   ,CreateTime
                                   ,CreateUserID
                                   ,ROW_NUMBER() OVER (ORDER BY CreateTime DESC) AS [RowNumber] 
                             FROM GoodsBaseInfo G WITH (NOLOCK) JOIN CategoryInfo C WITH (NOLOCK)
                             ON G.Category=C.ID 
                            WHERE G.IsDelete=0 AND C.IsDelete=0 {0}
                        )
                        SELECT * FROM Virtual_T 
                        WHERE @PageSize * (@CurrentPage - 1) < RowNumber AND RowNumber <= @PageSize * @CurrentPage
                    ";

            //条件查询部分
            var sqlWhere = "";
            var parameters = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(keyWords))
            {
                sqlWhere += "AND GoodsName LIKE @GoodsName";
                parameters.Add(new SqlParameter() { ParameterName = "@GoodsName", Value = "%" + keyWords + "%" });
            }
            sql = string.Format(sql, sqlWhere);
            parameters.Add(new SqlParameter() { ParameterName = "@PageSize", Value = pageSize });
            parameters.Add(new SqlParameter() { ParameterName = "@CurrentPage", Value = currentPage });

            //记录总数计算
            var countParameters = new List<SqlParameter>();
            parameters.ForEach(h => countParameters.Add(new SqlParameter() { ParameterName = h.ParameterName, Value = h.Value }));
            var sqlCount = string.Format("SELECT COUNT(*) CNT FROM [GoodsBaseInfo] WHERE IsDelete=0 {0} ", sqlWhere);
            allCount = Converter.TryToInt32(SqlHelper.ExecuteScalar(sqlCount, countParameters.ToArray()));

            if (allCount == 0)
            {
                return null;
            }

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
                }).ToList();
            }
            return null;
        }
        #endregion
    }
}
