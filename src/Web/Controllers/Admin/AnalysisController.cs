using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.ViewServices;
using Web.Models;
using Microsoft.Extensions.Options;
using ApplicationCore.Settings;
using ApplicationCore.Specifications;

namespace Web.Controllers.Admin
{
	public class AnalysisController : BaseAdminController
	{
		private readonly IQuestionsService _questionsService;
		private readonly IRecruitsService _recruitsService;
		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly RootSubjectSettings _rootSubjectSettings;
		private readonly IMapper _mapper;

		public AnalysisController(IQuestionsService questionsService, IRecruitsService recruitsService,
			IOptions<RootSubjectSettings> rootSubjectSettings, ISubjectsService subjectsService, ITermsService termsService, IMapper mapper)
		{
			_questionsService = questionsService;
			_recruitsService = recruitsService;
			_subjectsService = subjectsService;
			_termsService = termsService;
			_rootSubjectSettings = rootSubjectSettings.Value;
			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index()
		{
			var subjects = await _subjectsService.FetchExamSubjectsAsync();
			var recruits = await _recruitsService.FetchAsync(parentId: 0);
			var model = new AnalysisIndexModel
			{
				Subjects = subjects.MapViewModelList(_mapper),
				Recruits = recruits.MapViewModelList(_mapper),
				RootSubjectSettings = _rootSubjectSettings
			};
			return Ok(model);
		}

		[HttpGet("rq")]
		public async Task<ActionResult> RQ(int subject)
		{
			var rootSubject = _subjectsService.GetById(subject);
			if (rootSubject == null) return NotFound();
			if (rootSubject.ParentId > 0)
			{
				ModelState.AddModelError("subject", "錯誤的科目");
				return BadRequest(ModelState);
			}

			var rootSubjectView = rootSubject.MapViewModel(_mapper);
			var subjectViews = rootSubject.SubItems.MapViewModelList(_mapper);

			var allTerms = await _termsService.FetchAsync(new TermFilterBySubjectsSpecification(rootSubject.GetSubIds().ToList()));

			var allRecruits = await _recruitsService.GetAllAsync();
			var recruits = allRecruits.Where(x => x.SubjectId == subject).ToList();

			var models = new List<ExamSettingsViewModel>();

			foreach (var recruit in recruits)
			{
				recruit.LoadParent(allRecruits);
				recruit.LoadSubItems(allRecruits);

				//取得一個recruit年度的試題
				var rqModel = await GetRQModelAsync(recruit, rootSubject);

				var model = new ExamSettingsViewModel(rootSubjectView, recruit.MapViewModel(_mapper));
				foreach (var rqPart in rqModel.Parts)
				{
					
					var partSettings = new ExamPartSettings()
					{
						Order = rqPart.Order,
						Title = rqPart.Title,
						Points = rqPart.Points,
						OptionCount = rqPart.OptionCount,
						OptionType = rqPart.OptionType,
						MultiAnswers = rqPart.MultiAnswers,

						Questions = rqPart.Questions.Count
					};


					var subjectSettings = new List<ExamSubjectSettings>();
					foreach (var subjectView in subjectViews)
					{
						var terms = allTerms.Where(x => x.SubjectId == subjectView.Id && x.ParentId == 0).GetOrdered();
						foreach (var term in terms) term.LoadSubItems(allTerms);

						var termViews = terms.MapViewModelList(_mapper);

						var examSubjectSettings = new ExamSubjectSettings(subjectView, termViews);
						subjectSettings.Add(examSubjectSettings);
					}

					partSettings.Subjects = subjectSettings;

					foreach (var question in rqPart.Questions)
					{
						partSettings.AddQuestion(question);
					}

					model.Parts.Add(partSettings);

				}

				models.Add(model);

			}


			var indexModel = new AnalysisIndexModel();
			indexModel.Results = models;
			indexModel.ViewList = models.MapAnalysisViewList();


			return Ok(indexModel);
		}

		async Task<RQViewModel> GetRQModelAsync(Recruit recruit, Subject subject)
		{
			var model = new RQViewModel();

			var parts = recruit.SubItems;

			if (parts.HasItems())
			{
				foreach (var part in parts)
				{
					var questions = await _questionsService.FetchByRecruitAsync(part, subject);
					var partView = new RQPartViewModel
					{ 
						Points = part.Points,
						Order = part.Order,
						MultiAnswers = part.MultiAnswers,
						OptionCount = part.OptionCount,
						OptionType = part.OptionType.ToString(),
						Title = part.Title
					};
					partView.Questions = questions.MapViewModelList(_mapper);
					model.Parts.Add(partView);
				}

			}
			else
			{
				var questions = await _questionsService.FetchByRecruitAsync(recruit, subject);

				var partView = new RQPartViewModel
				{
					Points = recruit.Points,
					MultiAnswers = recruit.MultiAnswers,
					OptionCount = recruit.OptionCount,
					OptionType = recruit.OptionType.ToString(),
					Title = recruit.Title

				};
				partView.Questions = questions.MapViewModelList(_mapper);
				model.Parts.Add(partView);
			}

			model.Recruit = recruit.MapViewModel(_mapper);
			model.LoadTitle();

			return model;
		}

		[HttpPost("exam-settings")]
		public async Task<ActionResult> Store([FromBody] ExamSettingsViewModel model)
		{
			//Save ExamSettings

			var subjectId = model.SubjectId;

			return Ok();
		}

		[HttpGet("__rq")]
		public async Task<ActionResult> __RQ(int subject)
		{
			var allSubjects = await _subjectsService.FetchAsync();
			var selectedSubject = allSubjects.FirstOrDefault(x => x.Id == subject);
			selectedSubject.LoadSubItems(allSubjects);

			var allRecruits = await _recruitsService.GetAllAsync();
			var selectedRecruits = allRecruits.Where(x => x.SubjectId == subject);

			var results = new List<RecruitQuestionAnalysisView>();

			foreach (var recruit in selectedRecruits)
			{
				recruit.LoadParent(allRecruits);
				recruit.LoadSubItems(allRecruits);

				var model = new RecruitQuestionAnalysisView() { RecruitId = recruit.Id, SubjectId = selectedSubject .Id };

				var parts = recruit.SubItems;
				if (parts.HasItems())
				{
					foreach (var part in parts)
					{
						var questions = (await _questionsService.FetchByRecruitAsync(part, selectedSubject)).ToList();
						var pointsPerQuestion = part.Points / questions.Count;

						var details = questions.Select(q => new QuestionAnalysisDetailView
						{
							QuestionId = q.Id,
							SubjectId = q.SubjectId,
							Points = pointsPerQuestion
						});

						model.Details.AddRange(details);
					}

				}
				else
				{
					var questions = (await _questionsService.FetchByRecruitAsync(recruit, selectedSubject)).ToList();
					var pointsPerQuestion = 100 / questions.Count;

					var details = questions.Select(q => new QuestionAnalysisDetailView
					{
						QuestionId = q.Id,
						SubjectId = q.SubjectId,
						Points = pointsPerQuestion
					});

					model.Details.AddRange(details);
				}

				model.LoadSummaryList();
				results.Add(model);

			}

			
			return Ok(results.MapToViewModelList(selectedRecruits.ToList(), allSubjects.ToList(), _mapper));
		}

		
	}
}
