﻿using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationCore.Views
{
	public class TermViewModel : BaseCategoryView
	{
		public int Id { get; set; }

		public bool ChapterTitle { get; set; }

		public int SubjectId { get; set; }

		public SubjectViewModel Subject { get; set; }

		public string Text { get; set; }

		public string Highlight { get; set; } //json string

		public string Reference { get; set; } //json string

		public ICollection<string> Highlights { get; set; } = new List<string>();

		public ICollection<ReferenceViewModel> References { get; set; } = new List<ReferenceViewModel>();

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

	public class ReferenceViewModel
	{
		public string Id { get; set; }
		public string Text { get; set; }
	}
}
