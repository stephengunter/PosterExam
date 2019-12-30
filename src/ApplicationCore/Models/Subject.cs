using Infrastructure.Entities;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using ApplicationCore.Helpers;

namespace ApplicationCore.Models
{
	public class Subject : BaseCategory
	{
		public ICollection<Term> Terms { get; set; }

		public ICollection<Question> Questions { get; set; }
		

		[NotMapped]
		public ICollection<Subject> SubItems { get; private set; }


		public void LoadSubItems(IEnumerable<Subject> subItems)
		{
			SubItems = subItems.Where(item => item.ParentId == this.Id).OrderBy(item => item.Order).ToList();
			
			foreach (var item in SubItems)
			{
				item.LoadSubItems(subItems);
			}	
		}
	}
}
