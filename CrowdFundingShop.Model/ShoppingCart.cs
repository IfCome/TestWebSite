using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.Model
{
    public class ShoppingCart
    {
        public long ID { get; set; }
        public long ConsumerID { get; set; }
        public long HuoDongID { get; set; }
        public int StoreCount { get; set; }
        public int ZhongChouCount { get; set; }
        public string ShowIcons { get; set; }
        public string Price { get; set; }
        public long GoodsID { get; set; }
        public string GoodsName { get; set; }
        public double ZhongChouPercent { get; set; }
        /// <summary>
        /// 判断属于哪种类型数据进入 1、直接点击购物车或者购买页面进入 2、从结算页面直接过来
        /// </summary>
        public int Type { get; set; }
    }
}
