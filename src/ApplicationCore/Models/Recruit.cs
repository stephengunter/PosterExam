using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace ApplicationCore.Models
{
	public class Recruit : BaseRecord
	{
		public int Year { get; set; }

		public string Title { get; set; }

		public DateTime? Date { get; set; }

		public bool Done { get; set; }

		public ICollection<RecruitQuestion> RecruitQuestions { get; set; }
		
	}
}
