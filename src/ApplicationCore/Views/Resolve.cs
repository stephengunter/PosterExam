using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Views
{
    public class ResolveViewModel : BaseRecordView
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public string Text { get; set; }

        public string Highlight { get; set; } //json string

        public string Source { get; set; } //json string

        public bool Reviewed { get; set; }

        public ICollection<SourceViewModel> Sources { get; set; }

        public ICollection<string> Highlights { get; set; }
    }


    public class SourceViewModel
    { 
        public string Link { get; set; }
        public string Text { get; set; }
    }

}
