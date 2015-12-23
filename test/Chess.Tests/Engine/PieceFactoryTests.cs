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
	public class PieceFactoryTests
	{
		[Fact]
		public void CreateTest()
		{
			Assert.IsType<King>(PieceFactory.Create(Color.White, "Ka1"));
			Assert.IsType<Queen>(PieceFactory.Create(Color.White, "Qa1"));
			Assert.IsType<Bishop>(PieceFactory.Create(Color.White, "Ba1"));
			Assert.IsType<Knight>(PieceFactory.Create(Color.White, "Na1"));
			Assert.IsType<Rook>(PieceFactory.Create(Color.White, "Ra1"));
			Assert.IsType<Pawn>(PieceFactory.Create(Color.White, "pa1"));

			Assert.Throws<ArgumentException>(() => PieceFactory.Create(Color.White, "Da1"));
		}
	}
}