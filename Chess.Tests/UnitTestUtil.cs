using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Tests
{
	public static class UnitTestUtil
	{
		public static void AssertThrows<TException>(Action action) where TException : Exception
		{
			try
			{
				action();
			}
			catch(Exception e)
			{
				if(e.GetType() == typeof(TException))
				{
					return;
				}
				throw;
			}

			Assert.Fail($"Expected '{typeof(TException).Name}' exception but none was thrown");
		}
	}
}
