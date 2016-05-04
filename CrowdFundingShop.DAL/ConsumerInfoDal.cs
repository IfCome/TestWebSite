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
            int i = SqlHelper.ExecuteNonQuery(sql, parameters.ToArray());
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
        #endregion
    }
}
