using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Collections;

namespace CrowdFundingShop.Utility
{
    public class JsonEntityExchange<T> where T : new()
    {
        /// <summary>
        /// 将json动态转化成实体
        /// </summary>
        /// <param name="jsonStr">json串</param>
        /// <returns></returns>
        public static T ConvertJsonToEntity(string jsonStr)
        {
            try
            {
                JavaScriptSerializer Jss = new JavaScriptSerializer();
                PropertyInfo[] propinfos = null;
                T entity = new T();
                Dictionary<string, object> respDic = (Dictionary<string, object>)Jss.DeserializeObject(jsonStr);
                foreach (var o in respDic)
                {
                    //初始化propertyinfo
                    if (propinfos == null)
                    {
                        Type objtype = entity.GetType();
                        propinfos = objtype.GetProperties();
                    }
                    foreach (PropertyInfo pi in propinfos)
                    {
                        if (respDic.ContainsKey(pi.Name) && pi.Name == o.Key)
                        {
                            pi.SetValue(entity, Convert.ChangeType(o.Value, pi.PropertyType), null);
                        }
                    }
                }
                return entity;
            }
            catch(Exception ex)
            {
                //CommonMethod.WriteTxt_Log(ex);
                return default(T);
            }
            
        }


        /// <summary>
        /// 实体转json
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string ConvertEntityToJson(object entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(entity);
        }
    }
}
