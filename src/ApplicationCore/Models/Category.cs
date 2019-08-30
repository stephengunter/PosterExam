using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace ApplicationCore.Models
{
	public class Category : BaseCategory, IAggregateRoot
	{
		public CategoryType Type { get; set; }
	}

	public enum CategoryType
	{
		Subject,
		Term
	}
}
