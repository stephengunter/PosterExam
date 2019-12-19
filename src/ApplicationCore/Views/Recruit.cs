﻿using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Views
{
	public class RecruitViewModel : BaseRecordView
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "請填寫年度")]
		public int Year { get; set; }

		[Required(ErrorMessage = "請填寫標題")]
		public string Title { get; set; }
	}
}
