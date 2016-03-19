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

        #region 查询

        /// <summary>
        /// 通过UserName和password查询实体（不存在时，返回null）
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
                            AND [PassWord] = @PassWord
                            AND [IsDelete] = 0
                    ";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@UserName", Value = userName });
            parameters.Add(new SqlParameter() { ParameterName = "@PassWord", Value = pwd });

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


        /// <summary>
        /// 通过ID查询实体（不存在时，返回null）
        /// </summary>
        /// <param name="databaseConnectionString">数据库链接字符串</param>
        /// <param name="wherePart">条件部分</param>
        public static Model.BackgroundUserInfo GetUserInfoByID(long id)
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
                            [ID] = @ID
                            AND [IsDelete] = 0
                    ";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@ID", Value = id });

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

        /// <summary>
        /// 获取指定用户名的用户量
        /// </summary>
        public static int GetCountByUserName(string userName)
        {
            var sql = @"
                        SELECT COUNT(*) FROM [BackgroundUserInfo] WITH (NOLOCK)
                        WHERE 
                            [UserName] = @UserName
                    ";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@UserName", Value = userName });

            var count = SqlHelper.ExecuteScalar(sql, parameters.ToArray());

            return Converter.TryToInt32(count);
        }


        /// <summary>
        /// 根据指定条件分页查询用户数据
        /// </summary>
        /// <param name="pageSize">每页数据量</param>
        /// <param name="currentPage">页码</param>
        /// <param name="roleType">角色</param>
        /// <param name="allCount">用户总量</param>
        /// <param name="keyWords">搜索关键字</param>
        public static List<Model.BackgroundUserInfo> GetPageListByCondition(int pageSize, int currentPage, int roleType, out int allCount, string keyWords = "")
        {
            var sql = @"
                        WITH Virtual_T AS
                        (
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
                                    ,[IsDelete]
                                    ,ROW_NUMBER() OVER (ORDER BY LastLoginTime DESC) AS [RowNumber] 
	                        FROM [BackgroundUserInfo] WITH (NOLOCK)
                            {0}
                        )
                        SELECT * FROM Virtual_T 
                        WHERE @PageSize * (@CurrentPage - 1) < RowNumber AND RowNumber <= @PageSize * @CurrentPage
                    ";

            //条件查询部分
            var sqlWhere = " WHERE IsDelete=0 ";
            var parameters = new List<SqlParameter>();
            if (roleType == 10 || roleType == 20)
            {
                sqlWhere += " AND RoleType=@RoleType ";
                parameters.Add(new SqlParameter() { ParameterName = "@RoleType", Value = roleType });
            }
            if (keyWords.IsNullOrWhiteSpace() == false)
            {
                sqlWhere += " AND (RealName LIKE @Name OR UserName LIKE @Name ) ";
                parameters.Add(new SqlParameter() { ParameterName = "@Name", Value = "%" + keyWords + "%" });
            }

            sql = string.Format(sql, sqlWhere);
            parameters.Add(new SqlParameter() { ParameterName = "@PageSize", Value = pageSize });
            parameters.Add(new SqlParameter() { ParameterName = "@CurrentPage", Value = currentPage });

            //记录总数计算
            var countParameters = new List<SqlParameter>();
            parameters.ForEach(h => countParameters.Add(new SqlParameter() { ParameterName = h.ParameterName, Value = h.Value }));
            var sqlCount = string.Format("SELECT COUNT(*) CNT FROM [BackgroundUserInfo] {0} ", sqlWhere);
            allCount = Converter.TryToInt32(SqlHelper.ExecuteScalar(sqlCount, countParameters.ToArray()));

            if (allCount == 0)
            {
                return null;
            }

            var dataTable = SqlHelper.ExecuteDataTable(sql, parameters.ToArray());
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.AsEnumerable().Select(row => new Model.BackgroundUserInfo()
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
                    CreateTime = Converter.TryToDateTime(row["CreateTime"], DateTime.MinValue),
                    LastLoginTime = Converter.TryToDateTime(row["LastLoginTime"], DateTime.MinValue),
                    IsDelete = Converter.TryToInt32(row["IsDelete"], -1),
                }).ToList();
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region 添加

        public static bool Add(Model.BackgroundUserInfo entity)
        {
            var sql = @"
                        INSERT INTO [BackgroundUserInfo]
                               (
                                [UserName]
                                ,[PassWord]
                                ,[RealName]
                                ,[RoleType]
                                ,[Phone]
                                ,[Email]
                                ,[QQ]
                                ,[HeadIcon]
                                ,[CreateTime]
                               )
                         VALUES
                               (
                                @UserName
                                ,@PassWord
                                ,@RealName
                                ,@RoleType
                                ,@Phone
                                ,@Email
                                ,@QQ
                                ,@HeadIcon
                                ,@CreateTime
                               )
                    ";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@UserName", Value = entity.UserName });
            parameters.Add(new SqlParameter() { ParameterName = "@PassWord", Value = entity.PassWord });
            parameters.Add(new SqlParameter() { ParameterName = "@RealName", Value = entity.RealName });
            parameters.Add(new SqlParameter() { ParameterName = "@RoleType", Value = entity.RoleType });
            parameters.Add(new SqlParameter() { ParameterName = "@Phone", Value = entity.Phone });
            parameters.Add(new SqlParameter() { ParameterName = "@Email", Value = entity.Email });
            parameters.Add(new SqlParameter() { ParameterName = "@QQ", Value = entity.QQ });
            parameters.Add(new SqlParameter() { ParameterName = "@HeadIcon", Value = entity.HeadIcon });
            parameters.Add(new SqlParameter() { ParameterName = "@CreateTime", Value = entity.CreateTime });

            int i = SqlHelper.ExecuteNonQuery(sql, parameters.ToArray());
            return i > 0 ? true : false;
        }

        #endregion

        #region 更新

        /// <summary>
        /// 通过ID更新LastLoginTime
        /// </summary>
        public static bool UpdateLastLoginTimeByID(DateTime lastLoginTime, long id)
        {
            var sql = @"
                        UPDATE [BackgroundUserInfo] SET [LastLoginTime]=@LastLoginTime
                        WHERE 
                            [ID] = @ID
                    ";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@LastLoginTime", Value = lastLoginTime });
            parameters.Add(new SqlParameter() { ParameterName = "@ID", Value = id });

            int i = SqlHelper.ExecuteNonQuery(sql, parameters.ToArray());
            return i > 0 ? true : false;
        }
        #endregion

        /// <summary>
        /// 通过ID更新实体
        /// </summary>
        public static bool UpdateByID(Model.BackgroundUserInfo entity)
        {
            var sql = @"
                        UPDATE [BackgroundUserInfo]
                            SET
                                [RealName] = @RealName
                                ,[RoleType] = @RoleType
                                ,[Phone] = @Phone
                                ,[Email] = @Email
                                ,[QQ] = @QQ
                                ,[HeadIcon] = @HeadIcon
                            WHERE 
                                [ID] = @ID
                    ";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@ID", Value = entity.ID });
            parameters.Add(new SqlParameter() { ParameterName = "@RealName", Value = entity.RealName });
            parameters.Add(new SqlParameter() { ParameterName = "@RoleType", Value = entity.RoleType });
            parameters.Add(new SqlParameter() { ParameterName = "@Phone", Value = entity.Phone });
            parameters.Add(new SqlParameter() { ParameterName = "@Email", Value = entity.Email });
            parameters.Add(new SqlParameter() { ParameterName = "@QQ", Value = entity.QQ });
            parameters.Add(new SqlParameter() { ParameterName = "@HeadIcon", Value = entity.HeadIcon });

            int i = SqlHelper.ExecuteNonQuery(sql, parameters.ToArray());
            return i > 0 ? true : false;
        }


    }
}
