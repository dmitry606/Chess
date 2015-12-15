﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chess.Engine
{
	internal static class ChessUtil
	{
		public static char ExtractCharType(string pieceString) => pieceString[0];
		public static string ExtractPosition(string pieceString) => pieceString.Substring(1, 2);
		public static string ComposePieceString(char charType, string position) => charType + position;

		public static Color Invert(this Color c)
		{
			return c == Color.White ? Color.Black : Color.White;
		}
	}
}
