using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.Model
{
    public class OrderInfo
    {
        public long ID { get; set; }
        public long ConsumerID { get; set; }
        public long HuodongID { get; set; }
        public int Number { get; set; }
        public DateTime CreateTime { get; set; }
        public int JiJiangJieXiao { get; set; }
        public int YiJieXiao { get; set; }
        public int CartCount { get; set; }
        public int StoreCount { get; set; }
        public string NickName { get; set; }
    }
}
