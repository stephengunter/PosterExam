using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace ApplicationCore.Models
{
	public class Option : BaseEntity
	{
		public string Title { get; set; }
		public bool Correct { get; set; }
		public int QuestionId { get; set; }


		public Question Question { get; set; }
	}
}
