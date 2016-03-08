using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.Model
{
    /// <summary>
    /// BackgroundUserInfo
    /// </summary>
    public class BackgroundUserInfo
    {

        /// <summary>
        /// 主键
        /// </summary>
        public long ID { get; set; }


        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }


        /// <summary>
        /// 登录密码 MD5加密
        /// </summary>
        public string PassWord { get; set; }


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


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }


        /// <summary>
        /// 最近一次登录时间
        /// </summary>
        public DateTime LastLoginTime { get; set; }


        /// <summary>
        /// 0:未被删除  1:已被删除
        /// </summary>
        public int IsDelete { get; set; }


    }

}
