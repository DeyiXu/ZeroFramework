using System;
using Microsoft.AspNetCore.Http;

namespace Zero.Web
{
    public static class ZeroHttpContext
    {
        public static IServiceProvider ServiceProvider;
        static ZeroHttpContext()
        {

        }

        public static HttpContext Current
        {
            get
            {
                object factory = ServiceProvider.GetService(typeof(Microsoft.AspNetCore.Http.IHttpContextAccessor));

                HttpContext context = ((IHttpContextAccessor)factory).HttpContext;
                return context;
            }
        }
    }
}
