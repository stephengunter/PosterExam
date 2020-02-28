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

namespace Web.Controllers.Admin
{
	public class AnalysisController : BaseAdminController
	{
		private readonly IQuestionsService _questionsService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly IRecruitsService _recruitsService;
		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly IMapper _mapper;

		public AnalysisController(IQuestionsService questionsService, IRecruitsService recruitsService, IAttachmentsService attachmentsService,
			ISubjectsService subjectsService, ITermsService termsService, IMapper mapper)
		{
			_questionsService = questionsService;
			_recruitsService = recruitsService;
			_attachmentsService = attachmentsService;
			_subjectsService = subjectsService;
			_termsService = termsService;

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
			};
			return Ok(model);
		}

		[HttpGet("rq")]
		public async Task<ActionResult> RQ(int subject)
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
