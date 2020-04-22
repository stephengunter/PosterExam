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

        IEnumerable<NoteCategoryViewModel> FetchNoteCategories();
        void SaveNoteCategories(IEnumerable<NoteCategoryViewModel> models);

        IEnumerable<TermViewModel> FetchTermNotesBySubject(int subjectId);
        TermViewModel FindTermNotesByTerm(int termId);
        void CleanTermNotes();
        void SaveTermNotes(TermViewModel model, List<NoteViewModel> noteViewList);

    }

    public class DataService : IDataService
    {
        private readonly IMongoRepository<ExamSettings> _examSettingsRepository;
        private readonly IMongoRepository<SubjectQuestions> _subjectQuestionsRepository;
        private readonly IMongoRepository<YearRecruit> _yearRecruitsRepository;
        private readonly IMongoRepository<NoteCategories> _noteCategoriesRepository;
        private readonly IMongoRepository<TermNotes> _termNotesRepository;

        public DataService(IMongoRepository<ExamSettings> examSettingsRepository, IMongoRepository<SubjectQuestions> subjectQuestionsRepository,
            IMongoRepository<YearRecruit> yearRecruitsRepository, IMongoRepository<NoteCategories> noteCategoriesRepository,
            IMongoRepository<TermNotes> termNotesRepository)
        {
            _examSettingsRepository = examSettingsRepository;
            _subjectQuestionsRepository = subjectQuestionsRepository;
            _yearRecruitsRepository = yearRecruitsRepository;
            _noteCategoriesRepository = noteCategoriesRepository;
            _termNotesRepository = termNotesRepository;
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

        public IEnumerable<NoteCategoryViewModel> FetchNoteCategories()
        {
            var docs = _noteCategoriesRepository.Fetch();
            if (docs.IsNullOrEmpty()) return null;

            return docs.Select(doc => JsonConvert.DeserializeObject<NoteCategoryViewModel>(doc.Content));
        }

        public void SaveNoteCategories(IEnumerable<NoteCategoryViewModel> models)
        {
            _noteCategoriesRepository.DeleteMany(x => true);

            var docs = models.Select(model => new NoteCategories { Content = JsonConvert.SerializeObject(model) });

            _noteCategoriesRepository.InsertMany(docs.ToList());
        }

        public IEnumerable<TermViewModel> FetchTermNotesBySubject(int subjectId)
        {
            var docs = _termNotesRepository.FilterBy(x => x.SubjectId == subjectId);
            if (docs.IsNullOrEmpty()) return new List<TermViewModel>();

            return docs.Select(doc => JsonConvert.DeserializeObject<TermViewModel>(doc.Content));
        }

        public TermViewModel FindTermNotesByTerm(int termId)
        {
            var doc = _termNotesRepository.FindOne(item => item.TermId == termId);
            if (doc == null) return null;

            return JsonConvert.DeserializeObject<TermViewModel>(doc.Content);
        }

        public void CleanTermNotes() => _termNotesRepository.DeleteMany(x => true);

        public void SaveTermNotes(TermViewModel model, List<NoteViewModel> noteViewList)
        {
            int termId = model.Id;
            int subjectId = model.SubjectId;

            model.Subject = null;
            if (model.SubItems.HasItems()) foreach (var item in model.SubItems) item.Subject = null;

            model.LoadNotes(noteViewList);


            _termNotesRepository.InsertOne(new TermNotes { SubjectId = subjectId, TermId = termId, Content = JsonConvert.SerializeObject(model) });


        }

    }
}
