using Chess.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Tests;
using Xunit;

namespace Chess.Engine.Tests
{
	public class ChessUtilTests
	{
		[Fact]
		public void ComposePieceStringTest()
		{
			Assert.Equal("Qe1", ChessUtil.ComposePieceString('Q', "e1"));
			Assert.Equal('Q', ChessUtil.ExtractCharType("Qe2"));
			Assert.Equal("e2", ChessUtil.ExtractPosition("Qe2"));
		}


	}
}