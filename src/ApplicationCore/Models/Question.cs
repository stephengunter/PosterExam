using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace ApplicationCore.Models
{
	public class Question : BaseRecord, IAggregateRoot
	{
		public int SubjectId { get; set; }
		public string Title { get; set; }
		public int TermId { get; set; }



		public Subject Subject { get; set; }
		public ICollection<Option> Options { get; set; } = new List<Option>();
	}
}
