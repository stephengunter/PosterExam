using ApplicationCore.Helpers;
using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Views
{
	public class ExamViewModel : BaseRecordView
	{
		public int Id { get; set; }
		public string ExamType { get; set; }
		public string RecruitExamType { get; set; }
		public string OptionType { get; set; }

		public int Year { get; set; }
		public int SubjectId { get; set; }


		public double Score { get; set; }
		public string UserId { get; set; }

		public ICollection<ExamPartViewModel> ExamParts { get; set; } = new List<ExamPartViewModel>();

		
	}

	public class ExamPartViewModel
	{
		public int Id { get; set; }
		public int ExamId { get; set; }
		public int OptionCount { get; set; }
		public double Points { get; set; }


		public ICollection<ExamQuestionViewModel> ExamQuestions { get; set; } = new List<ExamQuestionViewModel>();
	}
}
