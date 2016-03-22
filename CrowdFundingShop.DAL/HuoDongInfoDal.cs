using CrowdFundingShop.Utility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

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
    }
}
