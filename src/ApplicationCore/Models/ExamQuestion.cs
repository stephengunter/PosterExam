using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
		public string AnswerIndexes { get; set; }
		public string OptionIds { get; set; } // 12,4,34,15

		public string UserAnswerIndexes { get; set; }

		public ExamPart ExamPart { get; set; }
		public Question Question { get; set; }


		[NotMapped]
		public ICollection<Option> Options { get; set; }

	}
}
