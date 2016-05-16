using CrowdFundingShop.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.DAL
{
    public class CategoryInfoDal
    {
        #region 增加
        public static bool Add(Model.CategoryInfo entity)
        {
            var sql = @"
                        INSERT INTO [CategoryInfo]
                               (
                                    CategoryName
                                    ,ParentId
                                    ,IsDelete
                               )
                         VALUES
                               (
                                    @CategoryName
                                    ,@ParentId
                                    ,@IsDelete
                               )";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@CategoryName", Value = entity.CategoryName });
            parameters.Add(new SqlParameter() { ParameterName = "@ParentId", Value = entity.ParentId });
            parameters.Add(new SqlParameter() { ParameterName = "@IsDelete", Value = 0 });
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

        #region 查询
        public static List<Model.CategoryInfo> GetListByParentID(int parentID, string type)
        {
            var sql = @"
                                SELECT ID,ParentId,CategoryName 
                                FROM dbo.CategoryInfo 
                                WHERE IsDelete=0 {0}
                            ";
            var parameters = new List<SqlParameter>();
            string sqlWhere = string.Empty;
            if (type == "PC")
                if (parentID != 0)
                {
                    sqlWhere = " AND ParentID = @ParentId";
                    parameters.Add(new SqlParameter() { ParameterName = "@ParentId", Value = parentID });
                }
                else
                {
                    sqlWhere = " AND ParentID=0 AND ParentId IS NOT NULL";
                }
            else if (type == "WAP")
            {
                sqlWhere = " AND ParentID !=0 AND ParentID IS NOT NULL";
            }
            sql = string.Format(sql, sqlWhere);
            DataTable dataTable = SqlHelper.ExecuteDataTable(sql, parameters.ToArray());

            if (dataTable.Rows.Count > 0)
            {
                return dataTable.AsEnumerable().Select(row => new Model.CategoryInfo()
                {
                    ID = Converter.TryToInt32(row["ID"], -1),
                    ParentId = Converter.TryToInt32(row["ParentId"], -1),
                    CategoryName = Converter.TryToString(row["CategoryName"], string.Empty)
                }).ToList();
            }
            return null;
        }
        public static int GetCountByCateName(string catename)
        {
            var sql = @"SELECT COUNT(*) FROM CategoryInfo WHERE CategoryName=@CategoryName";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = "@CategoryName", Value = catename });

            var count = SqlHelper.ExecuteScalar(sql, parameters.ToArray());

            return Converter.TryToInt32(count);
        }
        #endregion
    }
}
