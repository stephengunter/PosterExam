using ApplicationCore.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ApplicationCore.Views
{
    public class ExamSettingsViewModel
    {
        public ExamSettingsViewModel() { }
        public ExamSettingsViewModel(SubjectViewModel subject, RecruitViewModel recruit = null)
        {
            SubjectId = subject.Id;
            Subject = subject;

            Recruit = recruit;
        }

        public RecruitViewModel Recruit { get; set; }
        public int SubjectId { get; set; }
        public List<ExamPartSettings> Parts { get; set; } = new List<ExamPartSettings>();


        public SubjectViewModel Subject { get; set; }
    }

    public class ExamPartSettings
    {
        public int Order { get; set; }
        public bool MultiAnswers { get; set; }
        public double Points { get; set; }
        public int Questions { get; set; }

        public void AddQuestion(QuestionViewModel question)
        {
            try
            {
                var subjectSettings = Subjects.FirstOrDefault(x => x.SubjectId == question.SubjectId);
                subjectSettings.AddQuestion(question);
            }
            catch (Exception)
            {
                throw new Exception("QuestionId: " + question.Id);
            }
           
        }

        public List<ExamSubjectSettings> Subjects { get; set; } = new List<ExamSubjectSettings>();
    }

    public class ExamSubjectSettings
    {
        public ExamSubjectSettings() { }
        public ExamSubjectSettings(SubjectViewModel subject, IEnumerable<TermViewModel> termViews)
        {
            SubjectId = subject.Id;
            Subject = subject;

            TermQuestions = termViews.Select(item => new ExamTermQuestionSettings(item)).ToList();
        }

        public void AddQuestion(QuestionViewModel question)
        {
            try
            {
                int termId = question.TermIds.SplitToIds().FirstOrDefault();
                var termQuestionSettings = FindTermQuestions(termId);
                termQuestionSettings.AddQuestion(question, termId);
            }
            catch (Exception)
            {
                throw new Exception("QuestionId: " + question.Id);
            }
        }

        public int SubjectId { get; set; }
        public SubjectViewModel Subject { get; set; }

        public int TotalQuestions
        {
            get
            {
                if (TermQuestions.IsNullOrEmpty()) return 0;
                return TermQuestions.Sum(item => item.TotalQuestions);

            }
        }

        public List<ExamTermQuestionSettings> TermQuestions { get; set; } = new List<ExamTermQuestionSettings>();


        ExamTermQuestionSettings FindTermQuestions(int termId)
        {
            if (TermQuestions.IsNullOrEmpty()) return null;

            var termQuestionSettings = TermQuestions.FirstOrDefault(x => x.TermId == termId);
            if (termQuestionSettings == null) termQuestionSettings = TermQuestions.FirstOrDefault(x => x.SubTermIds.Contains(termId));

            return termQuestionSettings;
        }
    }

    public class ExamTermQuestionSettings
    {
        public ExamTermQuestionSettings() { }
        public ExamTermQuestionSettings(TermViewModel termView)
        {
            TermId = termView.Id;
            Term = termView;

            if (termView.SubItems.HasItems())
            {
                SubItems = termView.SubItems.Select(item => new ExamTermQuestionSettings(item)).ToList();
                SubTermIds = termView.SubItems.Select(item => item.Id).ToList();
            }
        }

        public void AddQuestion(QuestionViewModel question, int termId)
        {
            try
            {
                if (termId == TermId) Questions += 1;
                else
                {
                    var subitem = SubItems.FirstOrDefault(x => x.TermId == termId);
                    subitem.Questions += 1;
                }
            }
            catch (Exception)
            {
                throw new Exception("QuestionId: " + question.Id);
            }
        }

        public int TermId { get; set; }
        public TermViewModel Term { get; set; }
        public int Questions { get; set; }

        public int TotalQuestions
        {
            get
            {
                if (SubItems.IsNullOrEmpty()) return Questions;
                return Questions + SubItems.Sum(item => item.Questions);

            }
        }

        public List<int> SubTermIds { get; set; } = new List<int>();

        public List<ExamTermQuestionSettings> SubItems { get; set; } = new List<ExamTermQuestionSettings>();
    }
}
