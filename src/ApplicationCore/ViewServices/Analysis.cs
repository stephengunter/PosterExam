using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Views;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using ApplicationCore.Helpers;
using System.Threading.Tasks;
using System.Linq;
using Infrastructure.Views;
using AutoMapper;

namespace ApplicationCore.ViewServices
{
    public static class AnalysisViewService
    {
        public static RecruitQuestionAnalysisView MapViewModel(this RecruitQuestionAnalysisView model,
            Recruit recruit, ICollection<Subject> subjects, IMapper mapper)
        {
            model.Recruit = recruit.MapViewModel(mapper);
            model.Subject = subjects.FirstOrDefault(x => x.Id == model.SubjectId).MapViewModel(mapper);

            var subjectIds = model.SummaryList.Select(item => item.SubjectId).ToList();
            var selectedSubjects = subjects.Where(x => subjectIds.Contains(x.Id));

            var subjectViews = selectedSubjects.MapViewModelList(mapper);

            foreach (var summary in model.SummaryList)
            {
                summary.Subject = subjectViews.FirstOrDefault(x => x.Id == summary.SubjectId);
            }

            return model;
        }

        public static List<RecruitQuestionAnalysisView> MapToViewModelList(this IEnumerable<RecruitQuestionAnalysisView> models,
            ICollection<Recruit> recruits, ICollection<Subject> subjects, IMapper mapper)
            => models.Select(item => MapViewModel(item, recruits.FirstOrDefault(x => x.Id == item.RecruitId), subjects, mapper)).ToList();

    }
}
