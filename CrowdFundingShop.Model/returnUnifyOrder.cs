using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.Model
{
   public class returnUnifyOrder
    {
       /// <summary>
        /// 公众账号ID
       /// </summary>
       public string appid { get; set; }
       /// <summary>
       /// 商户号
       /// </summary>
       public string mch_id { get; set; }
       /// <summary>
       /// 设备号
       /// </summary>
       public string device_info { get; set; }
       /// <summary>
       /// 随机字符串
       /// </summary>
       public string nonce_str { get; set; }
       /// <summary>
       /// 签名
       /// </summary>
       public string sign { get; set; }
       /// <summary>
       /// 业务结果
       /// </summary>
       public string result_code { get; set; }
       /// <summary>
       /// 错误代码
       /// </summary>
       public string err_code { get; set; }
       /// <summary>
       /// 错误代码描述
       /// </summary>
       public string err_code_des { get; set; }
       /// <summary>
       /// 交易类型
       /// </summary>
       public string trade_type { get; set; }
       /// <summary>
       /// 预支付交易会话标识
       /// </summary>
       public string prepay_id { get; set; }
       /// <summary>
       /// 二维码链接
       /// </summary>
       public string code_url { get; set; }
    }
}
