using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chess.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Tests;

namespace Chess.Tests.Models
{
	
	[TestClass]
	public class BoardMatrixTests
	{
		[TestMethod]
		public void ConvertToMatrixCoordsTest()
		{
			Assert.AreEqual(new Tuple<int, int>(7, 0), BoardMatrix.ConvertToTupleCoords("a1"));
			Assert.AreEqual(new Tuple<int, int>(7, 7), BoardMatrix.ConvertToTupleCoords("h1"));
			Assert.AreEqual(new Tuple<int, int>(0, 0), BoardMatrix.ConvertToTupleCoords("a8"));
			Assert.AreEqual(new Tuple<int, int>(0, 7), BoardMatrix.ConvertToTupleCoords("h8"));
			Assert.AreEqual(new Tuple<int, int>(4, 4), BoardMatrix.ConvertToTupleCoords("e4"));
			Assert.AreEqual(new Tuple<int, int>(3, 5), BoardMatrix.ConvertToTupleCoords("f5"));

			UnitTestUtil.AssertThrows<ArgumentException>(() =>
				BoardMatrix.ConvertToTupleCoords("ff5"));

			UnitTestUtil.AssertThrows<ArgumentException>(() =>
				BoardMatrix.ConvertToTupleCoords("f55"));

			UnitTestUtil.AssertThrows<ArgumentException>(() =>
				BoardMatrix.ConvertToTupleCoords("f9"));

			UnitTestUtil.AssertThrows<ArgumentException>(() =>
				BoardMatrix.ConvertToTupleCoords("aa"));

			UnitTestUtil.AssertThrows<ArgumentException>(() =>
				BoardMatrix.ConvertToTupleCoords("55"));

			UnitTestUtil.AssertThrows<ArgumentException>(() =>
				BoardMatrix.ConvertToTupleCoords("t4"));
		}

		[TestMethod()]
		public void ConvertToStringCoordsTest()
		{
			Assert.AreEqual("a1", BoardMatrix.ConvertCoords(7, 0));
			Assert.AreEqual("h1", BoardMatrix.ConvertCoords(7, 7));
			Assert.AreEqual("a8", BoardMatrix.ConvertCoords(0, 0));
			Assert.AreEqual("h8", BoardMatrix.ConvertCoords(0, 7));
			Assert.AreEqual("e4", BoardMatrix.ConvertCoords(4, 4));
			Assert.AreEqual("f5", BoardMatrix.ConvertCoords(3, 5));

			UnitTestUtil.AssertThrows<ArgumentOutOfRangeException>(() =>
				BoardMatrix.ConvertCoords(-1, 5));

			UnitTestUtil.AssertThrows<ArgumentOutOfRangeException>(() =>
				BoardMatrix.ConvertCoords(0, 8));

			UnitTestUtil.AssertThrows<ArgumentOutOfRangeException>(() =>
				BoardMatrix.ConvertCoords(8, 0));
		}

		[TestMethod()]
		public void IsValidCoordsTest()
		{
			Assert.IsFalse(BoardMatrix.IsValidCoords(new Tuple<int, int>(0, 8)));
			Assert.IsFalse(BoardMatrix.IsValidCoords(new Tuple<int, int>(8, 0)));
			Assert.IsFalse(BoardMatrix.IsValidCoords(new Tuple<int, int>(-1, 0)));
			Assert.IsFalse(BoardMatrix.IsValidCoords(new Tuple<int, int>(0, -1)));

			Assert.IsTrue(BoardMatrix.IsValidCoords(new Tuple<int, int>(0, 0)));
			Assert.IsTrue(BoardMatrix.IsValidCoords(new Tuple<int, int>(7, 7)));
		}

	}
}
