using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace ApplicationCore.Models
{
	public class ExamQuestion : BaseEntity, IAggregateRoot
	{
		public int ExamId { get; set; }
		public int Order { get; set; }
		public int QuestionId { get; set; }
		public int AnswerIndex { get; set; }
		public string OptionIndexes { get; set; } // 0,1,4,5

		public string UserAnswerIndex { get; set; }

		public Exam Exam { get; set; }
		public Question Question { get; set; }
	}
}
