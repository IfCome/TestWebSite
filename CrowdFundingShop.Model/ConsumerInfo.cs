using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.Model
{
    public class ConsumerInfo
    {
        public long ID { get; set; }
        public string WeiXinAccount { get; set; }
        public string NickName { get; set; }
        public string Phone { get; set; }
        public string HeadIcon { get; set; }
        public int JiJiangJieXiao { get; set; }
        public int YiJieXiao { get; set; }
        public int CartCount { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }

    }
}
