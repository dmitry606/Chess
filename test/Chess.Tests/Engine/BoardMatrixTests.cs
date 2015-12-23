using Chess.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Tests;
using Xunit;

namespace Chess.Tests.Models
{
	public class BoardMatrixTests
	{
		[Fact]
		public void ConvertToMatrixCoordsTest()
		{
			Assert.Equal(new Tuple<int, int>(7, 0), BoardMatrix.ConvertToTupleCoords("a1"));
			Assert.Equal(new Tuple<int, int>(7, 7), BoardMatrix.ConvertToTupleCoords("h1"));
			Assert.Equal(new Tuple<int, int>(0, 0), BoardMatrix.ConvertToTupleCoords("a8"));
			Assert.Equal(new Tuple<int, int>(0, 7), BoardMatrix.ConvertToTupleCoords("h8"));
			Assert.Equal(new Tuple<int, int>(4, 4), BoardMatrix.ConvertToTupleCoords("e4"));
			Assert.Equal(new Tuple<int, int>(3, 5), BoardMatrix.ConvertToTupleCoords("f5"));

			Assert.Throws<ArgumentException>(() =>
				BoardMatrix.ConvertToTupleCoords("ff5"));

			Assert.Throws<ArgumentException>(() =>
				BoardMatrix.ConvertToTupleCoords("f55"));

			Assert.Throws<ArgumentException>(() =>
				BoardMatrix.ConvertToTupleCoords("f9"));

			Assert.Throws<ArgumentException>(() =>
				BoardMatrix.ConvertToTupleCoords("aa"));

			Assert.Throws<ArgumentException>(() =>
				BoardMatrix.ConvertToTupleCoords("55"));

			Assert.Throws<ArgumentException>(() =>
				BoardMatrix.ConvertToTupleCoords("t4"));
		}

		[Fact]
		public void ConvertToStringCoordsTest()
		{
			Assert.Equal("a1", BoardMatrix.ConvertCoords(7, 0));
			Assert.Equal("h1", BoardMatrix.ConvertCoords(7, 7));
			Assert.Equal("a8", BoardMatrix.ConvertCoords(0, 0));
			Assert.Equal("h8", BoardMatrix.ConvertCoords(0, 7));
			Assert.Equal("e4", BoardMatrix.ConvertCoords(4, 4));
			Assert.Equal("f5", BoardMatrix.ConvertCoords(3, 5));

			Assert.Throws<ArgumentOutOfRangeException>(() =>
				BoardMatrix.ConvertCoords(-1, 5));

			Assert.Throws<ArgumentOutOfRangeException>(() =>
				BoardMatrix.ConvertCoords(0, 8));

			Assert.Throws<ArgumentOutOfRangeException>(() =>
				BoardMatrix.ConvertCoords(8, 0));
		}

		[Fact]
		public void IsValidCoordsTest()
		{
			Assert.False(BoardMatrix.IsValidCoords(new Tuple<int, int>(0, 8)));
			Assert.False(BoardMatrix.IsValidCoords(new Tuple<int, int>(8, 0)));
			Assert.False(BoardMatrix.IsValidCoords(new Tuple<int, int>(-1, 0)));
			Assert.False(BoardMatrix.IsValidCoords(new Tuple<int, int>(0, -1)));

			Assert.True(BoardMatrix.IsValidCoords(new Tuple<int, int>(0, 0)));
			Assert.True(BoardMatrix.IsValidCoords(new Tuple<int, int>(7, 7)));
		}

	}
}
