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
    }
}
