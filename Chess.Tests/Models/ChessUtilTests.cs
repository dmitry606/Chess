using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Tests;

namespace Chess.Models.Tests
{
	[TestClass()]
	public class ChessUtilTests
	{

		[TestMethod()]
		public void ComposePieceStringTest()
		{
			Assert.AreEqual("Qe1", ChessUtil.ComposePieceString('Q', "e1"));
			Assert.AreEqual('Q', ChessUtil.ExtractCharType("Qe2"));
			Assert.AreEqual("e2", ChessUtil.ExtractPosition("Qe2"));
		}


	}
}