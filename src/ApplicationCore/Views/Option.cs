using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Text;


namespace ApplicationCore.Views
{
	public class OptionViewModel : BaseRecordView
	{
		public string Title { get; set; }
		public bool Correct { get; set; }
		public int QuestionId { get; set; }

	}
}
