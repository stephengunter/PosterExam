using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Views
{
	public class QuestionViewModel : BaseRecordView
	{
		public int Id { get; set; }
		public int SubjectId { get; set; }
		public string Title { get; set; }
		public int TermId { get; set; }

		public string OptionsText { get; set; }

		public TermViewModel Term  { get; set; }
		public ICollection<OptionViewModel> Options { get; set; } = new List<OptionViewModel>();
		public ICollection<RecruitViewModel> Recruits { get; set; } = new List<RecruitViewModel>();

		public string RecruitsText { get; set; }
	}

	public class QuestionEditForm
	{
		public QuestionViewModel Question { get; set; } = new QuestionViewModel();

		public ICollection<TermViewModel> Terms { get; set; }
	}

}
