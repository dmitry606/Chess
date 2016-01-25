using Chess.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.MvcClient.Infrastructure
{
    public interface IUserInfo
    {
		string AuthString { get; set; }
    }

	public class UserInfo : IUserInfo
	{
		public string AuthString { get; set; }
	}
}
