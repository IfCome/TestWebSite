using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.BLL
{
    public class CategoryInfoBll
    {
        public static bool Add(Model.CategoryInfo entity)
        {
            return DAL.CategoryInfoDal.Add(entity);
        }
        public static List<Model.CategoryInfo> GetListByParentID(int parentID, string type)
        {
            return DAL.CategoryInfoDal.GetListByParentID(parentID,type);
        }
    }
}
