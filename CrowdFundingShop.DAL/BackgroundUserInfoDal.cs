using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CrowdFundingShop.Utility;

namespace CrowdFundingShop.DAL
{
    public static class BackgroundUserInfoDal
    {
        /// <summary>
        /// 通过UserName查询实体（不存在时，返回null）
        /// </summary>
        /// <param name="databaseConnectionString">数据库链接字符串</param>
        /// <param name="wherePart">条件部分</param>
        public static Model.BackgroundUserInfo GetInfoByUserNameAndPwd(string userName, string pwd)
        {
            var sql = @"
                        SELECT
                                [ID]
                                ,[UserName]
                                ,[PassWord]
                                ,[RealName]
                                ,[RoleType]
                                ,[Phone]
                                ,[Email]
                                ,[QQ]
                                ,[HeadIcon]
                                ,[CreateTime]
                                ,[LastLoginTime]
                         
                        FROM [BackgroundUserInfo] WITH (NOLOCK)
                        WHERE 
                            [UserName] = @UserName
                            AND [IsDelete] = 0
                    ";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@UserName", Value = userName });

            var dataTable = SqlHelper.ExecuteDataTable(sql, parameters.ToArray());

            if (dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];
                return new Model.BackgroundUserInfo()
                {
                    ID = Converter.TryToInt64(row["ID"], -1),
                    UserName = Converter.TryToString(row["UserName"], string.Empty),
                    PassWord = Converter.TryToString(row["PassWord"], string.Empty),
                    RealName = Converter.TryToString(row["RealName"], string.Empty),
                    RoleType = Converter.TryToInt32(row["RoleType"], -1),
                    Phone = Converter.TryToString(row["Phone"], string.Empty),
                    Email = Converter.TryToString(row["Email"], string.Empty),
                    QQ = Converter.TryToString(row["QQ"], string.Empty),
                    HeadIcon = Converter.TryToString(row["HeadIcon"], string.Empty),
                    CreateTime = Converter.TryToDateTime(row["CreateTime"], Convert.ToDateTime("1900-01-01")),
                    LastLoginTime = Converter.TryToDateTime(row["LastLoginTime"], Convert.ToDateTime("1900-01-01"))
                };
            }
            else
            {
                return null;
            }
        }


    }
}
