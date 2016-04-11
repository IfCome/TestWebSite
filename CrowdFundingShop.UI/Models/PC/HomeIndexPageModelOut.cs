using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrowdFundingShop.UI.Models.PC
{
    public class HomeIndexPageModelOut
    {
        /// <summary>
        /// 活动ID
        /// </summary>
        public long GoodsID { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 活动期数
        /// </summary>
        public int HuodongNumber { get; set; }

        /// <summary>
        /// 最新参与者
        /// </summary>
        public string LastestCustomer { get; set; }

        /// <summary>
        /// 众筹总数
        /// </summary>
        public int ShareCount { get; set; }

        /// <summary>
        /// 已众筹数
        /// </summary>
        public int OrderCount { get; set; }

        /// <summary>
        /// 众筹进度（百分比）
        /// </summary>
        public float Progress { get; set; }

        /// <summary>
        /// 日增量
        /// </summary>
        public int DailyIncrease { get; set; }
    }
}