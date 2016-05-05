using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using WeixinCommonApi;

namespace CrowdFundingShop.Utility
{
   public class JsIncomHelp
    {
       /// <summary>
       /// 处理js需求数据和签名
       /// </summary>
       /// <param name="entity"></param>
       /// <param name="key"></param>
       /// <returns></returns>
       public string DoDataForIncomeJs(JsIncomeModel entity,string key)
       {
           string jsonStr = null;
           string[] signTemp = { "appId=" + entity.appId, "timeStamp=" + entity.timeStamp, "nonceStr=" + entity.nonceStr, "package=" + entity.package, "signType=" + entity.signType };
           List<string> signList = signTemp.ToList();
           #region 处理支付签名
           //对signList按照ASCII码从小到大的顺序排序
           signList.Sort();

           string signOld = string.Empty;
           string payForWeiXinOld = string.Empty;
           int i = 0;
           foreach (string temp in signList)
           {
               signOld += temp + "&";
               i++;
           }
           signOld = signOld.Substring(0, signOld.Length - 1);
           //拼接Key
           signOld += "&key=" + key;
           //处理支付签名
           entity.paySign = Encrypt(signOld).ToUpper();
           #endregion
           jsonStr = JsonEntityExchange<JsIncomeModel>.ConvertEntityToJson(entity);
           return jsonStr;
       }
       public string DoDataForsign(JsIncomeModel entity, string key)
       {
           string jsonStr = null;
           string[] signTemp = { "appId=" + entity.appId, "timeStamp=" + entity.timeStamp, "nonceStr=" + entity.nonceStr, "package=" + entity.package, "signType=" + entity.signType };
           List<string> signList = signTemp.ToList();
           #region 处理支付签名
           //对signList按照ASCII码从小到大的顺序排序
           signList.Sort();

           string signOld = string.Empty;
           string payForWeiXinOld = string.Empty;
           int i = 0;
           foreach (string temp in signList)
           {
               signOld += temp + "&";
               i++;
           }
           signOld = signOld.Substring(0, signOld.Length - 1);
           //拼接Key
           signOld += "&key=" + key;
           //处理支付签名
           jsonStr = Encrypt(signOld).ToUpper();
           #endregion
           return jsonStr;
       }


       /// <summary>
       /// Md5加密
       /// </summary>
       /// <param name="s"></param>
       /// <returns></returns>
       public static String Encrypt(String s)
       {
           MD5 md5 = new MD5CryptoServiceProvider();
           byte[] bytes = System.Text.Encoding.UTF8.GetBytes(s);
           bytes = md5.ComputeHash(bytes);
           md5.Clear();
           string ret = "";
           for (int i = 0; i < bytes.Length; i++)
           {
               ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
           }
           return ret.PadLeft(32, '0');
       }
    }
}
