using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.Model
{
    public class HuoDongInfo
    {
        public long ID { get; set; }
        public long GoodsID { get; set; }
        public int ShareCount { get; set; }
        public int State { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime FinishedTime { get; set; }
        public int HuodongNumber { get; set; }
        public long LuckDogID { get; set; }
        public int LuckNumber { get; set; }
        public int IsDelete { get; set; }
    }
}
