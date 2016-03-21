using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrowdFundingShop.UI.Models.PC
{
    public class BackGroundUserInfoModelIn
    {
        private long id;
        /// <summary>
        /// 用户ID
        /// </summary>
        public long ID
        {
            get { return id; }
            set { id = value; }
        }

        private string userName;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get
            {
                var temp = String.IsNullOrEmpty(userName) ? string.Empty : userName;
                temp = HttpUtility.UrlDecode(temp);
                return temp;
            }
            set { userName = value; }
        }

        private string password;
        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password
        {
            get
            {
                var temp = String.IsNullOrEmpty(password) ? string.Empty : password;
                temp = HttpUtility.UrlDecode(temp);
                return temp;
            }
            set { password = value; }
        }


        private string rePassword;
        /// <summary>
        /// 登录密码
        /// </summary>
        public string RePassword
        {
            get
            {
                var temp = String.IsNullOrEmpty(rePassword) ? string.Empty : rePassword;
                temp = HttpUtility.UrlDecode(temp);
                return temp;
            }
            set { rePassword = value; }
        }

        private string realName;
        /// <summary>
        /// 正式姓名
        /// </summary>
        public string RealName
        {
            get
            {
                var temp = String.IsNullOrEmpty(realName) ? string.Empty : realName;
                temp = HttpUtility.UrlDecode(temp);
                return temp;
            }
            set { realName = value; }
        }

        private int roleType;
        /// <summary>
        /// 角色类型（10:管理员 20:普通员工）
        /// </summary>
        public int RoleType { get; set; }

        private string phone;
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone
        {
            get
            {
                var temp = String.IsNullOrEmpty(phone) ? string.Empty : phone;
                temp = HttpUtility.UrlDecode(temp);
                return temp;
            }
            set { phone = value; }
        }

        private string email;
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email
        {
            get
            {
                var temp = String.IsNullOrEmpty(email) ? string.Empty : email;
                temp = HttpUtility.UrlDecode(temp);
                return temp;
            }
            set { email = value; }
        }

        private string qq;
        /// <summary>
        /// qq
        /// </summary>
        public string QQ
        {
            get
            {
                var temp = String.IsNullOrEmpty(qq) ? string.Empty : qq;
                temp = HttpUtility.UrlDecode(temp);
                return temp;
            }
            set { qq = value; }
        }

        private string headIcon;
        /// <summary>
        /// 头像
        /// </summary>
        public string HeadIcon
        {
            get
            {
                var temp = String.IsNullOrEmpty(headIcon) ? string.Empty : headIcon;
                temp = HttpUtility.UrlDecode(temp);
                return temp;
            }
            set { headIcon = value; }

        }
    }
}