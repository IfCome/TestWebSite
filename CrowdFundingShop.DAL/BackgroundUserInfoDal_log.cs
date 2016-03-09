using System;
using System.Collections.Generic;
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
    }
}
