using ApplicationCore.Settings;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using ApplicationCore.Models.Mongo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using Newtonsoft.Json;
using System.Linq;

namespace ApplicationCore.Services
{
    public interface IDataService
    {
        ExamSettingsViewModel FindExamSettings(int subjectId);
        void SaveExamSettings(int subjectId, ExamSettingsViewModel model);

        IEnumerable<SubjectQuestionsViewModel> FindSubjectQuestions(int subjectId);
        void SaveSubjectQuestions(int subjectId, IEnumerable<SubjectQuestionsViewModel> models);

        IEnumerable<RecruitViewModel> FetchYearRecruits();
        void SaveYearRecruits(IEnumerable<RecruitViewModel> models);
    }

    public class DataService : IDataService
    {
        private readonly IMongoRepository<ExamSettings> _examSettingsRepository;
        private readonly IMongoRepository<SubjectQuestions> _subjectQuestionsRepository;
        private readonly IMongoRepository<YearRecruit> _yearRecruitsRepository;

        public DataService(IMongoRepository<ExamSettings> examSettingsRepository, IMongoRepository<SubjectQuestions> subjectQuestionsRepository,
            IMongoRepository<YearRecruit> yearRecruitsRepository)
        {
            _examSettingsRepository = examSettingsRepository;
            _subjectQuestionsRepository = subjectQuestionsRepository;
            _yearRecruitsRepository = yearRecruitsRepository;
        }

        public void SaveExamSettings(int subjectId, ExamSettingsViewModel model)
        {
            var existingDoc = _examSettingsRepository.FindOne(item => item.SubjectId == subjectId);
            string content = JsonConvert.SerializeObject(model);

            if (existingDoc == null) _examSettingsRepository.InsertOne(new ExamSettings { SubjectId = subjectId, Content = content });
            else
            {

                existingDoc.Content = content;
                existingDoc.LastUpdated = DateTime.Now;
                _examSettingsRepository.ReplaceOne(existingDoc);
            }
        }

        public ExamSettingsViewModel FindExamSettings(int subjectId)
        {
            var doc = _examSettingsRepository.FindOne(item => item.SubjectId == subjectId);
            if (doc == null) return null;

            return JsonConvert.DeserializeObject<ExamSettingsViewModel>(doc.Content);

        }

        public IEnumerable<SubjectQuestionsViewModel> FindSubjectQuestions(int subjectId)
        {
            var doc = _subjectQuestionsRepository.FindOne(item => item.SubjectId == subjectId);
            if (doc == null) return null;

            return JsonConvert.DeserializeObject<IEnumerable<SubjectQuestionsViewModel>>(doc.Content);
        }

        public void SaveSubjectQuestions(int subjectId, IEnumerable<SubjectQuestionsViewModel> models)
        {
            var existingDoc = _subjectQuestionsRepository.FindOne(item => item.SubjectId == subjectId);
            string content = JsonConvert.SerializeObject(models);

            if (existingDoc == null) _subjectQuestionsRepository.InsertOne(new SubjectQuestions { SubjectId = subjectId, Content = content });
            else
            {

                existingDoc.Content = content;
                existingDoc.LastUpdated = DateTime.Now;
                _subjectQuestionsRepository.ReplaceOne(existingDoc);
            }
        }

        public IEnumerable<RecruitViewModel> FetchYearRecruits()
        {
            var docs = _yearRecruitsRepository.Fetch();
            if (docs.IsNullOrEmpty()) return null;

            return docs.Select(doc => JsonConvert.DeserializeObject<RecruitViewModel>(doc.Content));
        }

        public void SaveYearRecruits(IEnumerable<RecruitViewModel> models)
        {
            _yearRecruitsRepository.DeleteMany(x => true);

            var docs = models.Select(model => new YearRecruit { Content = JsonConvert.SerializeObject(model) });

            _yearRecruitsRepository.InsertMany(docs.ToList());
        }

    }
}
