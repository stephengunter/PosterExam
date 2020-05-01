using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayMvc.Helpers
{
    public static class CommonHelpers
    {
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
		{
			if (enumerable == null)
			{
				return true;
			}
			var collection = enumerable as ICollection<T>;
			if (collection != null)
			{
				return collection.Count < 1;
			}
			return !enumerable.Any();
		}

		public static bool HasItems<T>(this IEnumerable<T> enumerable) => !IsNullOrEmpty(enumerable);

		public static int ToInt(this string str)
		{
			int value = 0;
			if (!int.TryParse(str, out value)) value = 0;

			return value;
		}
	}
}