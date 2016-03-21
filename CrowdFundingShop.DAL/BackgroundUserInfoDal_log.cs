using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CrowdFundingShop.Utility;

namespace CrowdFundingShop.DAL
{
    public static class BackgroundUserInfoDal_log
    {
        public static bool Add(Model.BackgroundUserInfo_log entity)
        {
            var sql = @"
                        INSERT INTO [BackgroundUserInfo_log]
                               (
                                [UserID]
                                ,[OperateTile]
                                ,[OperateDetail]
                                ,[OperateTime]
                                ,[IpAddress]
                               )
                         VALUES
                               (
                                @UserID
                                ,@OperateTile
                                ,@OperateDetail
                                ,@OperateTime
                                ,@IpAddress
                               )
                    ";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@UserID", Value = entity.UserID });
            parameters.Add(new SqlParameter() { ParameterName = "@OperateTile", Value = entity.OperateTile });
            parameters.Add(new SqlParameter() { ParameterName = "@OperateDetail", Value = entity.OperateDetail });
            parameters.Add(new SqlParameter() { ParameterName = "@OperateTime", Value = entity.OperateTime });
            parameters.Add(new SqlParameter() { ParameterName = "@IpAddress", Value = entity.IpAddress });

            int i = SqlHelper.ExecuteNonQuery(sql, parameters.ToArray());
            return i > 0 ? true : false;
        }

        /// <summary>
        /// 通过UserID查询实体列表（不存在时，返回null）
        /// </summary>
        public static List<Model.BackgroundUserInfo_log> GetTop10ListByUserID(long userId)
        {
            var sql = @"
                        SELECT  TOP 10
                                [UserID]
                                ,[OperateTile]
                                ,[OperateDetail]
                                ,[OperateTime]
                                ,[IpAddress]                         
                        FROM [BackgroundUserInfo_log] WITH (NOLOCK)
                        WHERE 
                            [UserID] = @UserID
                        ORDER BY [OperateTime] DESC
                    ";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@UserID", Value = userId });

            var dataTable = SqlHelper.ExecuteDataTable(sql, parameters.ToArray());

            if (dataTable.Rows.Count > 0)
            {
                return dataTable.AsEnumerable().Select(row => new Model.BackgroundUserInfo_log()
                {
                    UserID = Converter.TryToInt64(row["UserID"], -1),
                    OperateTile = Converter.TryToString(row["OperateTile"], string.Empty),
                    OperateDetail = Converter.TryToString(row["OperateDetail"], string.Empty),
                    OperateTime = Converter.TryToDateTime(row["OperateTime"], Convert.ToDateTime("1900-01-01")),
                    IpAddress = Converter.TryToString(row["IpAddress"], string.Empty)

                }).ToList();
            }
            else
            {
                return null;
            }
        }
    }
}
