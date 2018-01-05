using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Zero.Json;

namespace Zero.Web.Session
{
    public class SessionHelper
    {
        #region Session操作
        /// <summary>
        /// 写Session
        /// </summary>
        /// <typeparam name="T">Session键值的类型</typeparam>
        /// <param name="key">Session的键名</param>
        /// <param name="value">Session的键值</param>
        public static void WriteSession<T>(string key, T value)
        {
            if (key.Trim() == string.Empty)
                return;
            ZeroHttpContext.Current.Session.SetString(key, value.ToJson());
        }

        /// <summary>
        /// 写Session
        /// </summary>
        /// <param name="key">Session的键名</param>
        /// <param name="value">Session的键值</param>
        public static void WriteSession(string key, string value)
        {
            WriteSession<string>(key, value);
        }

        /// <summary>
        /// 读取Session的值
        /// </summary>
        /// <param name="key">Session的键名</param>        
        public static string GetSession(string key)
        {
            key = key.Trim();
            if (key == string.Empty)
            {
                return string.Empty;
            }
            return ZeroHttpContext.Current.Session.GetString(key);
        }
        /// <summary>
        /// 读取Session的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Session的键名</param>
        /// <returns></returns>        
        public static T GetSession<T>(string key)
        {
            key = key.Trim();
            if (key == string.Empty)
            {
                return default(T);
            }
            return ZeroHttpContext.Current.Session.GetString(key).ToObject<T>();
        }
        /// <summary>
        /// 删除指定Session
        /// </summary>
        /// <param name="key">Session的键名</param>
        public static void RemoveSession(string key)
        {
            key = key.Trim();
            if (key == string.Empty)
            {
                return;
            }
            ZeroHttpContext.Current.Session.Remove(key);
        }

        #endregion
    }
}
