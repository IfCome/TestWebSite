using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CrowdFundingShop.Utility
{
    public static class Security
    {
        /// <summary>
        /// 获取字符串的md5
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>md5值</returns>
        public static string getMD5ByStr(string str)
        {
            StringBuilder sb = new StringBuilder();
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                bytes = md5.ComputeHash(bytes);
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("x2"));
                }
                md5.Clear();
            }
            return sb.ToString();
        }
        /// <summary>
        /// 获取文件的md5值
        /// </summary>
        /// <param name="path">文件的路径</param>
        /// <returns>md5值</returns>
        public static string getMD5ByFile(string path)
        {
            FileStream fs = File.OpenRead(path);
            StringBuilder sb = new StringBuilder();
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = md5.ComputeHash(fs);
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("x2"));
                }
                md5.Clear();
            }
            return sb.ToString();
        }

        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="input">待加密的字符串</param>
        /// <param name="password">加密的密码（只能为8位长）</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>加密之后的文本</returns>
        public static string Encrypt(string input, string password = "FSDPASSW", Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            //注意iv的长度，必须和key中的密码长度相同
            var iv = encoding.GetBytes(password);
            var key = encoding.GetBytes(password);
            var datas = encoding.GetBytes(input);
            var desCryptoServiceProvider = new DESCryptoServiceProvider();
            using (var memoryStream = new MemoryStream())
            {
                using (
                    var cryptoStream = new CryptoStream(memoryStream, desCryptoServiceProvider.CreateEncryptor(iv, key),
                                                        CryptoStreamMode.Write))
                {
                    cryptoStream.Write(datas, 0, datas.Length);
                    cryptoStream.FlushFinalBlock();
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }

        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="input">待解密的字符串</param>
        /// <param name="password">加密时用的密码（只能为8位长）</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>解密之后的文本</returns>
        public static string Decrypt(string input, string password = "FSDPASSW", Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            var iv = encoding.GetBytes(password);
            var key = encoding.GetBytes(password);
            var datas = Convert.FromBase64String(input);
            var desCryptoServiceProvider = new DESCryptoServiceProvider();
            using (var memoryStream = new MemoryStream())
            {
                using (
                    var cryptoStream = new CryptoStream(memoryStream, desCryptoServiceProvider.CreateDecryptor(iv, key),
                                                        CryptoStreamMode.Write))
                {
                    cryptoStream.Write(datas, 0, datas.Length);
                    cryptoStream.FlushFinalBlock();
                    return encoding.GetString(memoryStream.ToArray());
                }
            }
        }

        public static void SetUserLoginCookies(string userId, HttpResponseBase Response)
        {
            // 获取新的pairA，并计算对应的pairB
            string pairA = Guid.NewGuid().ToString();
            string pairB = Security.Encrypt(userId + pairA);
            // 加密用户ID
            userId = Security.Encrypt(userId);

            // 设置新的Cookies
            HttpCookie userIdCookie = new HttpCookie("ID", userId);
            userIdCookie.Expires = DateTime.Now.AddMinutes(30);

            HttpCookie pairACookie = new HttpCookie("PairA", pairA);
            pairACookie.Expires = DateTime.Now.AddMinutes(30);

            HttpCookie pairBCookie = new HttpCookie("PairB", pairB);
            pairBCookie.Expires = DateTime.Now.AddMinutes(30);

            Response.Cookies.Add(userIdCookie);
            Response.Cookies.Add(pairACookie);
            Response.Cookies.Add(pairBCookie);
        }
        /// <summary>
        /// 随机生成密码
        /// </summary>
        /// <returns></returns>
        public static string getPassword()
        {
            ArrayList list = new ArrayList();
            string str = "0,1,2,3,4,5,6,7,8,9";
            list.AddRange(str.Split(',')); //随机码。
            string randomContent = "";
            Random rd = new Random();
            //设置随机码的个数。这里你也可以随机生成一个范围数.
            int nuM = 8;
            for (int i = 0; i < nuM; i++)
            {
                randomContent += list[rd.Next(0, 9)];
            }
            return randomContent;
        }
    }
}
