using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrowdFundingShop.UI.Models.PC
{
    public class AddHuoDongInfoIn
    {
        public string ID { get; set; }
        public string GoodsID { get; set; }
        public string ShareCount { get; set; }
        public string State { get; set; }
        public string CreateTime { get; set; }
        public string CreateUser { get; set; }
        public string FinishedTime { get; set; }
        public string HuodongNumber { get; set; }
        public string LuckDogID { get; set; }
        public string LuckNumber { get; set; }
        public string IsDelete { get; set; }
    }
}