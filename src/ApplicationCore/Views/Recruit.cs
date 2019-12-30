using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Views;

namespace ApplicationCore.Views
{
	public class RecruitViewModel : BaseCategoryView
	{
		public int Id { get; set; }

		public int Year { get; set; }

		public int SubjectId { get; set; }

		public string DateText { get; set; }

		public bool Done { get; set; }

		public string DoneText => this.Done ? "已結束" : "";

		public ICollection<RecruitViewModel> SubItems { get; set; }

	}

	public class RecruitEditForm
	{
		public RecruitViewModel Recruit { get; set; } = new RecruitViewModel();

		public ICollection<RecruitViewModel> SubItems { get; set; } = new List<RecruitViewModel>();

		public ICollection<BaseOption<int>> SubjectOptions { get; set; }
	}
}
