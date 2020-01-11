using ApplicationCore.Helpers;
using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationCore.Views
{
	public class QuestionViewModel : BaseRecordView
	{
		public int Id { get; set; }
		public int SubjectId { get; set; }
		public string Title { get; set; }
		
		public bool MultiAnswers { get; set; }

		public string OptionsText { get; set; }
		public ICollection<OptionViewModel> Options { get; set; } = new List<OptionViewModel>();

		public ICollection<ResolveViewModel> Resolves { get; set; } = new List<ResolveViewModel>();

		public ICollection<RecruitViewModel> Recruits { get; set; } = new List<RecruitViewModel>();

		public string RecruitsText { get; set; }

		public string TermIds { get; set; }  //  Example: 1,6

		public ICollection<TermViewModel> Terms { get; set; }
	}

	public class QuestionEditForm
	{
		public QuestionViewModel Question { get; set; } = new QuestionViewModel();

		public ICollection<TermViewModel> Terms { get; set; }
	}

}
