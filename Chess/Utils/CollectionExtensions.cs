using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chess.Utils
{
	public static class CollectionExtensions
	{
		public static int SequenceGetHashCode<T>(this IEnumerable<T> coll)
		{
			if (null == coll)
				throw new ArgumentNullException(nameof(coll));

			unchecked
			{
				int hash = 19;
				foreach (var el in coll)
				{
					hash = hash * 31 + el.GetHashCode();
				}
				return hash;
			}
		}
	}
}