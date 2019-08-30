using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationCore.Helpers
{
	public static class EnumHelpers
	{
		public static List<T> ToList<T>() => Enum.GetValues(typeof(T)).Cast<T>().ToList();
	}
}
