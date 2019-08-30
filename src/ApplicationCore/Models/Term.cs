using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace ApplicationCore.Models
{
	public class Term : BaseCategory, IAggregateRoot
	{
		public int SubjectId { get; set; }

		public string Text { get; set; }

		public Subject Subject { get; set; }

		[NotMapped]
		public ICollection<Term> SubItems { get; private set; }


		public void LoadSubItems(IEnumerable<Term> subItems)
		{
			SubItems = subItems.Where(item => item.ParentId == this.Id).OrderBy(item => item.Order).ToList();

			foreach (var item in SubItems)
			{
				item.LoadSubItems(subItems);
			}
		}
	}
}
