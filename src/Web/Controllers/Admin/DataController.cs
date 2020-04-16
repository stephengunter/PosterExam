using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using Web.Controllers;
using ApplicationCore.ViewServices;

namespace Web.Controllers.Admin
{
	public class DataController : BaseAdminController
	{
		private readonly AdminSettings _adminSettings;
		private readonly IDataService _dataService;
		private readonly RootSubjectSettings _rootSubjectSettings;

		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly IRecruitsService _recruitsService;
		private readonly IQuestionsService _questionsService;

		private readonly IMapper _mapper;

		public DataController(IOptions<RootSubjectSettings> rootSubjectSettings, IOptions<AdminSettings> adminSettings,
			IDataService dataService, ISubjectsService subjectsService, ITermsService termsService, IRecruitsService recruitsService,
			IQuestionsService questionsService, IMapper mapper)
		{
			_adminSettings = adminSettings.Value;
			_rootSubjectSettings = rootSubjectSettings.Value;
			_subjectsService = subjectsService;
			_termsService = termsService;
			_recruitsService = recruitsService;
			_questionsService = questionsService;

			_dataService = dataService;

			_mapper = mapper;
		}


		//儲存每個Subject, Term 底下的QuestionId(精選試題)
		#region subject-questions
		
		[HttpPost("subject-questions")]
		public async Task<ActionResult> StoreSubjectQuestions(AdminRequest model)
		{
			ValidateRequest(model, _adminSettings);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			//獲取所有歷屆試題Id
			var recruitQuestionIds = _questionsService.FetchAllRecruitQuestionIds();

			//專業科目(1)：臺灣自然及人文地理
			var firstRootSubject = _subjectsService.GetById(_rootSubjectSettings.FirstId);
			await SaveSubjectQuestionsAsync(firstRootSubject, recruitQuestionIds);


			//專業科目(2)：郵政法規大意及交通安全常識
			var secondRootSubject = _subjectsService.GetById(_rootSubjectSettings.SecondId);
			await SaveSubjectQuestionsAsync(secondRootSubject, recruitQuestionIds);


			return Ok();
		}

		async Task SaveSubjectQuestionsAsync(Subject rootSubject, IEnumerable<int> recruitQuestionIds)
		{
			var subjects = rootSubject.SubItems;

			var models = new List<SubjectQuestionsViewModel>();
			foreach (var subject in subjects)
			{
				int parentId = 0;
				var terms = await _termsService.FetchAsync(subject, parentId);
				_termsService.LoadSubItems(terms);

				var subjectQuestionsModel = new SubjectQuestionsViewModel { SubjectId = subject.Id };
				foreach (var term in terms)
				{
					var termQuestionsModel = new TermQuestionsViewModel
					{
						TermId = term.Id,
						QuestionIds = term.QIds.SplitToIds()
					};
					
					if (term.SubItems.HasItems())
					{
						termQuestionsModel.SubItems = term.SubItems.Select(subItem => new TermQuestionsViewModel
						{
							TermId = subItem.Id,
							QuestionIds = subItem.QIds.SplitToIds()
						}).ToList();
					}

					subjectQuestionsModel.TermQuestions.Add(termQuestionsModel);

				}

				models.Add(subjectQuestionsModel);
			}

			_dataService.SaveSubjectQuestions(rootSubject.Id, models);
		}
		#endregion


		[HttpPost("year-recruits")]
		public async Task<ActionResult> StoreYearRecruits(AdminRequest model)
		{
			ValidateRequest(model, _adminSettings);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			int parent = 0;
			var recruits = await _recruitsService.FetchAsync(parent);

			recruits = recruits.Where(x => x.Active).GetOrdered();

			_recruitsService.LoadSubItems(recruits);

			var recruitViews = recruits.MapViewModelList(_mapper);

			foreach (var yearView in recruitViews)
			{
				foreach (var recruitView in yearView.SubItems)
				{
					recruitView.QuestionIds = _questionsService.FetchQuestionIdsByRecruit(new Recruit { Id = recruitView.Id }).ToList();

					foreach (var partView in recruitView.SubItems)
					{
						partView.QuestionIds = _questionsService.FetchQuestionIdsByRecruit(new Recruit { Id = partView.Id }).ToList();
					}
				}

			}


			_dataService.SaveYearRecruits(recruitViews);

			return Ok();
		}
	}
}
