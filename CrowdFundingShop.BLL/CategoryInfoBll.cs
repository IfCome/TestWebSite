using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.BLL
{
    public class CategoryInfoBll
    {
        public static List<Model.CategoryInfo> GetListByParentID(int parentID)
        {
            return DAL.CategoryInfoDal.GetListByParentID(parentID);
        }
    }
}
