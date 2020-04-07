using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ApplicationCore.Helpers;

namespace ApplicationCore.Views
{

    public class SubjectQuestionsViewModel
    { 
        public int SubjectId { get; set; }
        public ICollection<TermQuestionsViewModel> TermQuestions { get; set; } = new List<TermQuestionsViewModel>();

        public List<int> GetQuestionIds()
        { 
            if(TermQuestions.IsNullOrEmpty()) return new List<int>();
            return TermQuestions.SelectMany(item => item.GetQuestionIds()).Distinct().ToList();
        }
    }

    public class TermQuestionsViewModel
    {
        public int TermId { get; set; }
        public ICollection<int> QuestionIds { get; set; } = new List<int>();

        public List<int> GetQuestionIds()
        {
            if (SubItems.IsNullOrEmpty()) return QuestionIds.ToList();

            var subQids = SubItems.SelectMany(item => item.GetQuestionIds());
            if(subQids.IsNullOrEmpty()) return QuestionIds.ToList();

            return QuestionIds.Union(subQids).Distinct().ToList();

        }
        

        public ICollection<TermQuestionsViewModel> SubItems { get; set; }
    }
}
