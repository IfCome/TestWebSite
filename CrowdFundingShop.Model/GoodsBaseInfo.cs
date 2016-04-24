using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.Model
{
    public class GoodsBaseInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }
        /// <summary>
        /// 商品详情
        /// </summary>
        public string Describe { get; set; }
        /// <summary>
        /// 商品价格
        /// </summary>
        public string Price { get; set; }
        /// <summary>
        /// 商品分类
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 商品分类具体信息
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 展示图片
        /// </summary>
        public string ShowIcons { get; set; }
        /// <summary>
        /// 商品详情图片(逗号分割多个图片地址)
        /// </summary>
        public string DetailIcons { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 创建者ID
        /// </summary>
        public string CreateUserID { get; set; }
        /// <summary>
        /// 创建者ID
        /// </summary>
        public int IsDelete { get; set; }
        /// <summary>
        /// 众筹状态0未众筹;10众筹中;20待开奖;30已开奖;
        /// </summary>
        public int State { get; set; }

        public string ParentId { get; set; }

        public int ZhongChouCount { get; set; }

        public double ZhongChouPercent { get; set; }
        
        public long HuoDongID { get; set; }
    }
}
