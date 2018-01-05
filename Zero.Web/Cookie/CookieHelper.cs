using Microsoft.AspNetCore.Http;
using System;

namespace Zero.Web.Cookie
{
    public class CookieHelper
    {
        #region Cookie操作
        /// <summary>
        /// 写cookie值,默认7天
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string strValue)
        {
            WriteCookie(strName, strValue, 10080);
        }
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="strValue">过期时间(分钟)</param>
        public static void WriteCookie(string strName, string strValue, int expires)
        {
            strName = strName.Trim();
            strValue = strValue.Trim();
            CookieOptions cookie = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(expires)
            };

            ZeroHttpContext.Current.Response.Cookies.Append(strName, strValue, cookie);
        }
        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName)
        {
            strName = strName.Trim();
            if (ZeroHttpContext.Current.Request.Cookies != null && ZeroHttpContext.Current.Request.Cookies[strName] != null)
            {
                return ZeroHttpContext.Current.Request.Cookies[strName];
            }
            return string.Empty;
        }
        /// <summary>
        /// 删除Cookie对象
        /// </summary>
        /// <param name="CookiesName">Cookie对象名称</param>
        public static void RemoveCookie(string CookiesName)
        {
            CookiesName = CookiesName.Trim();
            if (ZeroHttpContext.Current.Request.Cookies.ContainsKey(CookiesName))
            {
                ZeroHttpContext.Current.Response.Cookies.Delete(CookiesName.Trim());
            }
        }
        #endregion
    }
}
