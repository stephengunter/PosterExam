using ApplicationCore.Helpers;
using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Views
{
    public class NoteViewModel : BaseCategoryView
    {
        public int Id { get; set; }

        public int TermId { get; set; }

        public string Text { get; set; }

        public bool Important { get; set; }

        public string Highlight { get; set; } //json string

        public string Reference { get; set; } //json string

        public ICollection<AttachmentViewModel> Attachments { get; set; } = new List<AttachmentViewModel>();

        public ICollection<ReferenceViewModel> References { get; set; } = new List<ReferenceViewModel>();

        public ICollection<string> Highlights { get; set; } = new List<string>();
    }
}
