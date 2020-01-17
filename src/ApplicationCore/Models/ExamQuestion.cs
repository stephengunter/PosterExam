using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace ApplicationCore.Models
{
	public class ExamQuestion : BaseEntity
	{
		public int ExamPartId { get; set; }
		public int Order { get; set; }
		public int QuestionId { get; set; }
		public int AnswerIndex { get; set; }
		public string OptionIds { get; set; } // 12,4,34,15

		public string UserAnswerIndex { get; set; }

		public ExamPart ExamPart { get; set; }
		public Question Question { get; set; }
	}
}
