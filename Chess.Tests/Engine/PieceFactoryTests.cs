using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chess.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Tests;

namespace Chess.Engine.Tests
{
	[TestClass()]
	public class PieceFactoryTests
	{
		[TestMethod()]
		public void CreateTest()
		{
			Assert.IsInstanceOfType(PieceFactory.Create(Color.White, "Ka1"), typeof(King));
			Assert.IsInstanceOfType(PieceFactory.Create(Color.White, "Qa1"), typeof(Queen));
			Assert.IsInstanceOfType(PieceFactory.Create(Color.White, "Ba1"), typeof(Bishop));
			Assert.IsInstanceOfType(PieceFactory.Create(Color.White, "Na1"), typeof(Knight));
			Assert.IsInstanceOfType(PieceFactory.Create(Color.White, "Ra1"), typeof(Rook));
			Assert.IsInstanceOfType(PieceFactory.Create(Color.White, "pa1"), typeof(Pawn));

			UnitTestUtil.AssertThrows<ArgumentException>(() => PieceFactory.Create(Color.White, "Da1"));
		}
	}
}