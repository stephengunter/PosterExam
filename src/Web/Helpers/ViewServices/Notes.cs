using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;
using ApplicationCore.Models;

namespace Web.Helpers
{
    public static class NotesViewService
    {
		public static NoteCategoryViewModel MapNoteCategoryViewModel(this Subject subject, int parentId = 0)
		{
			return new NoteCategoryViewModel
			{
				 Id = subject.Id,
				 ParentId = parentId,
				 Text = subject.Title,
				 Type = subject.ParentId > 0 ? NoteCategoryType.Subject.ToString() : NoteCategoryType.Root.ToString()

			};
		}

		public static NoteCategoryViewModel MapNoteCategoryViewModel(this Term term)
		{
			return new NoteCategoryViewModel
			{
				Id = term.Id,
				ParentId = term.SubjectId,
				Text = $"{term.Title} {term.Text}",
				Type = NoteCategoryType.ChapterTitle.ToString()

			};
		}
	}
}
