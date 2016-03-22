using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrowdFundingShop.UI.Models.PC
{
    public class AddGoodsCallBackIn
    {
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
        /// 展示图片(逗号分割多个图片地址)
        /// </summary>
        public string ShowIcons { get; set; }
        /// <summary>
        /// 商品详情图片(逗号分割多个图片地址)
        /// </summary>
        public string DetailIcons { get; set; }
        /// <summary>
        /// 创建者ID
        /// </summary>
        public string CreateUserID { get; set; }
    }
}