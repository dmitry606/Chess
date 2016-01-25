using Chess.Engine;
using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.MvcClient.Infrastructure
{
    public static class CookieHelper
    {
		public const string AuthCookieKey = "ngChess";

		public static string GetAuthCookie(HttpRequest request)
		{
			return request.Cookies[AuthCookieKey];
		}

		public static void SetAuthCookie(HttpResponse response, string authString)
		{
			response.Cookies.Append(AuthCookieKey, authString);
		}

		public static void RemoveAuthCookie(HttpResponse response)
		{
			response.Cookies.Delete(AuthCookieKey);
		}
    }
}
