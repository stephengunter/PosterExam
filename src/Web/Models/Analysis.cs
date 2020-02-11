using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Views;

namespace Web.Models
{
    public class AnalysisIndexModel
    {
        public ICollection<SubjectViewModel> Subjects { get; set; }

        public ICollection<RecruitViewModel> Recruits { get; set; }
    }
}
