using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Views
{
	public class QuestionViewModel : BaseRecordView
	{
		public int SubjectId { get; set; }
		public string Title { get; set; }
		public int TermId { get; set; }

		public string OptionsText { get; set; }

		public TermViewModel Term  { get; set; }
		public ICollection<OptionViewModel> Options { get; set; } = new List<OptionViewModel>();
	}

}
