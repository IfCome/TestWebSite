using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrowdFundingShop.UI.Models.PC
{
    public class GetConsumerInfoListIn
    {
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public string KeyWords { get; set; }
    }
}