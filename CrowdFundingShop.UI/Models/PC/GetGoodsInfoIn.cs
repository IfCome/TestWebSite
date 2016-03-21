using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrowdFundingShop.UI.Models.PC
{
    public class GetGoodsInfoIn
    {
        /// <summary>
        /// 每页显示多少条
        /// </summary>
        public string PageIndex { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public string CurrentPage { get; set; }
    }
}