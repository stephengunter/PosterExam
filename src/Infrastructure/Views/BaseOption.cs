using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Views
{
	public class BaseOption
	{
		public BaseOption(string value, string text)
		{
			this.Value = value;
			this.Text = text;
		}
		public string Value { get; set; }
		public string Text { get; set; }
	}
}
