using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ApplicationCore.Services
{
	public abstract class BaseCategoriesService<T> where T : BaseCategory
	{
		protected IEnumerable<T> AllSubItems(DbSet<T> categoryDbSet) => categoryDbSet.Where(item => !item.Removed && item.ParentId > 0);
	}
}
