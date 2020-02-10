﻿using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using ApplicationCore.Views;
using Infrastructure.Views;


namespace Web.Models
{
    public enum NoteCategoryType
    {
        Root = 0,
        Subject = 1,
        ChapterTitle = 2
    }

    public class NoteCategoryViewModel
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public ICollection<NoteCategoryViewModel> SubItems { get; set; } = new List<NoteCategoryViewModel>();
    }
    public class NoteAdminViewModel
    {
        public List<NoteCategoryViewModel> Categories { get; set; } = new List<NoteCategoryViewModel>();


        public SubjectViewModel Subject { get; set; }
        //index
        public List<SubjectViewModel> RootSubjects { get; set; } = new List<SubjectViewModel>();

        public List<SubjectViewModel> Subjects { get; set; } = new List<SubjectViewModel>();

        public List<TermViewModel> Terms { get; set; } = new List<TermViewModel>();
    }
}
