using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using ApplicationCore.Helpers;
using System.Linq;

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

		public void LoadOptions()
		{
			this.Options = new List<Option>();
			var ids = OptionIds.SplitToIds();
			for (int i = 0; i < ids.Count; i++)
			{
				var item = Question.Options.FirstOrDefault(x => x.Id == ids[i]);
				this.Options.Add(item);
			}

		}

	}
}
