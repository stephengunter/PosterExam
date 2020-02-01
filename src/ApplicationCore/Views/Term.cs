using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationCore.Views
{
	public class TermViewModel : BaseCategoryView
	{
		public int Id { get; set; }

		public int SubjectId { get; set; }

		public SubjectViewModel Subject { get; set; }

		public string Text { get; set; }

		public string FullText => $"{Title} {Text}";

		public ICollection<TermViewModel> SubItems { get; set; }

		public ICollection<int> ParentIds { get; set; }

		public ICollection<NoteViewModel> Notes { get; set; }

		public void LoadNotes(IEnumerable<NoteViewModel> notes)
		{
			Notes = notes.Where(item => item.TermId == this.Id).OrderBy(item => item.Order).ToList();

			foreach (var item in SubItems)
			{
				item.LoadNotes(notes);
			}
		}
	}

	public class TermEditForm
	{
		public TermViewModel Term { get; set; } = new TermViewModel();

		public ICollection<TermViewModel> Parents { get; set; }
	}
}
