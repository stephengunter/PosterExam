using ApplicationCore.Views;
using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Helpers;
using System.Linq;
using ApplicationCore.Models;
using ApplicationCore.ViewServices;
using Web.Models;

namespace Web.Helpers
{
    public static class ExamsViewService
    {
        public static void LoadSubjectOptions(this ExamIndexViewModel model, IEnumerable<Subject> subjects, string emptyText = "")
        {
            var options = subjects.Select(item => item.ToOption()).ToList();

            if (!String.IsNullOrEmpty(emptyText)) options.Insert(0, new BaseOption<int>(-1, emptyText));

            model.SubjectOptions = options;

        }

        public static void LoadStatusOptions(this ExamIndexViewModel model, string emptyText = "全部")
        {
            var options = GetStatusOptions().ToList();

            if (!String.IsNullOrEmpty(emptyText)) options.Insert(0, new BaseOption<int>(-1, emptyText));

            model.StatusOptions = options;
        }

        public static ICollection<BaseOption<int>> GetStatusOptions()
        {
            var options = new List<BaseOption<int>>();
            foreach (ExamStaus status in (ExamStaus[])Enum.GetValues(typeof(ExamStaus)))
            {
                string text = status.GetDisplayName();
                if (!String.IsNullOrEmpty(text))
                {
                    options.Add(new BaseOption<int>((int)status, text));
                }

            }
            return options;
        }

        public static string GetDisplayName(this ExamStaus status)
        {
            if (status == ExamStaus.Reserved) return "未完成";
            if (status == ExamStaus.Completed) return "已完成";
            return "";
        }

        public static ExamStaus ToExamStaus(this int val)
        {
            try
            {

                if (Enum.IsDefined(typeof(ExamStaus), val))
                {
                    ExamStaus status = (ExamStaus)val;
                    return status;
                }
                return ExamStaus.Unknown;


            }
            catch (Exception ex)
            {
                return ExamStaus.Unknown;
            }
        }

        public static IEnumerable<Exam> FilterByStatus(this IEnumerable<Exam> exams, ExamStaus staus)
        {
            if (staus == ExamStaus.Completed) return exams.Where(x => x.IsComplete);
            if (staus == ExamStaus.Reserved) return exams.Where(x => x.Reserved);
            return exams;

        }
    }
}
