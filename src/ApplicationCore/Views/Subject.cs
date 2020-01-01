using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Views
{
	public class SubjectViewModel : BaseCategoryView
	{
		public int Id { get; set; }

		public ICollection<SubjectViewModel> SubItems { get; set; }

		public ICollection<int> SubIds { get; set; }
	}

	public class SubjectEditForm
	{
		public SubjectViewModel Subject { get; set; } = new SubjectViewModel();

		public ICollection<SubjectViewModel> Parents { get; set; }
	}
}
