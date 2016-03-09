using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.Model
{
    public class BackgroundUserInfo_log
    {

        /// <summary>
        /// 后台用户ID
        /// </summary>
        public long UserID { get; set; }


        /// <summary>
        /// 操作类型
        /// </summary>
        public string OperateTile { get; set; }


        /// <summary>
        /// 操作详情
        /// </summary>
        public string OperateDetail { get; set; }


        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperateTime { get; set; }


        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; }
    }
}
