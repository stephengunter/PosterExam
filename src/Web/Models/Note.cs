using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using ApplicationCore.Views;
using Infrastructure.Views;


namespace Web.Models
{
   

    public class NoteAdminViewModel
    {
        public SubjectViewModel Subject { get; set; }
        //index
        public List<SubjectViewModel> RootSubjects { get; set; } = new List<SubjectViewModel>();

        public List<SubjectViewModel> Subjects { get; set; } = new List<SubjectViewModel>();

        public List<TermViewModel> Terms { get; set; } = new List<TermViewModel>();
    }
}
