using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrowdFundingShop.UI.Models.PC
{
    public class BackGroundUserInfoModelIn
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }


        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }


        /// <summary>
        /// 登录密码
        /// </summary>
        public string RePassword { get; set; }

        /// <summary>
        /// 正式姓名
        /// </summary>
        public string RealName { get; set; }


        /// <summary>
        /// 角色类型（10:管理员 20:普通员工）
        /// </summary>
        public int RoleType { get; set; }


        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }


        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }


        /// <summary>
        /// qq
        /// </summary>
        public string QQ { get; set; }


        /// <summary>
        /// 头像
        /// </summary>
        public string HeadIcon { get; set; }
    }
}