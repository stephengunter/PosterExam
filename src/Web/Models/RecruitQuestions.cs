using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using ApplicationCore.Views;
using Infrastructure.Views;

namespace Web.Models
{
    public enum RQMode
    {
        Read = 0,
        Exam = 1,
        Unknown = -1
    }

    public class RQPartViewModel
    { 
        public int Points { get; set; }

        public string Title { get; set; }

        public PagedList<Question, QuestionViewModel> Questions { get; set; }
    }

    public class RQIndexViewModel
    {
        public ICollection<BaseOption<int>> ModeOptions { get; set; } = new List<BaseOption<int>>();

        public ICollection<BaseOption<int>> YearOptions { get; set; } = new List<BaseOption<int>>();

        public ICollection<RecruitViewModel> Subjects { get; set; } = new List<RecruitViewModel>();

        public ICollection<RQPartViewModel> Parts { get; set; } = new List<RQPartViewModel>();

        
    }
}
