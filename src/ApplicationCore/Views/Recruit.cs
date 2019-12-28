using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Views
{
	public class RecruitViewModel : BaseRecordView
	{
		public int Id { get; set; }

		public int Year { get; set; }

		public string Title { get; set; }

		public string DateText { get; set; }

		public bool Done { get; set; }

		public string DoneText => this.Done ? "已結束" : "";
		
	}
}
