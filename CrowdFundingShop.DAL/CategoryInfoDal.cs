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
        #region 查询
        public static List<Model.CategoryInfo> GetListByParentID(int parentID)
        {
            var sql = @"
                                SELECT ID,ParentId,CategoryName 
                                FROM dbo.CategoryInfo 
                                WHERE IsDelete=0 {0}
                            ";
            var parameters = new List<SqlParameter>();
            string sqlWhere = string.Empty;
            if (parentID != 0)
            {
                sqlWhere = " AND ParentID = @ParentId";
                parameters.Add(new SqlParameter() { ParameterName = "@ParentId", Value = parentID });
            }
            else
            {
                sqlWhere = " AND ParentId !=0 AND ParentId IS NOT NULL";
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
        #endregion
    }
}
