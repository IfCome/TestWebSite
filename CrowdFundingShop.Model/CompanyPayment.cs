using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.Model
{
    public class CompanyPayment
    {
        public string mch_appid { get; set; }
        public string mchid { get; set; }
        public string nonce_str { get; set; }
        public string partner_trade_no { get; set; }
        public string openid { get; set; }
        public string check_name { get; set; }
        public string amount { get; set; }
        public string desc { get; set; }
        public string spbill_create_ip { get; set; }
        public string device_info { get; set; }
        public string re_user_name { get; set; }
        public string sign { get; set; }
    }
}
