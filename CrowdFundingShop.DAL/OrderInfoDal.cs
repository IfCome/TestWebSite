using CrowdFundingShop.Utility;
using System;
using System.Collections.Generic;
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
        #endregion
        #region 删
        #endregion
        #region 改
        #endregion
        #region 查
        #endregion
    }
}
