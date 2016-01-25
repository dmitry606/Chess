using Chess.MvcClient.Repositories;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.MvcClient.Infrastructure
{
    public class AuthCookiesMiddleware
    {
		private readonly RequestDelegate _next;

		public AuthCookiesMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public Task Invoke(HttpContext context)
		{
			var authString = CookieHelper.GetAuthCookie(context.Request);
			var userInfo = (IUserInfo)context.RequestServices.GetService(typeof(IUserInfo));

			if (!string.IsNullOrEmpty(authString))
			{
				userInfo.AuthString = authString;
			}
			else
			{
				authString = Guid.NewGuid().ToString("N");
				CookieHelper.SetAuthCookie(context.Response, authString);
				userInfo.AuthString = authString;
			}

			return _next.Invoke(context);
		}
    }
}
