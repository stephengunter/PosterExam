using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Views
{
	public class TermViewModel : BaseCategoryView
	{
		public int Id { get; set; }

		public int SubjectId { get; set; }

		public string Text { get; set; }

		public string FullText => $"{Title} {Text}";

		public ICollection<TermViewModel> SubItems { get; set; }

		public ICollection<int> ParentIds { get; set; }

	}

	public class TermEditForm
	{
		public TermViewModel Term { get; set; } = new TermViewModel();

		public ICollection<TermViewModel> Parents { get; set; }
	}
}
