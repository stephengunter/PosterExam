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
using Microsoft.AspNetCore.Authorization;
using Web.Models;
using Web.Helpers;

namespace Web.Controllers.Api
{
	public class RecruitQuestionsController : BaseController
	{
		private readonly IQuestionsService _questionsService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly IRecruitsService _recruitsService;
		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly IExamsService _examsService;
		private readonly IMapper _mapper;

		public RecruitQuestionsController(IQuestionsService questionsService, IRecruitsService recruitsService,
			IAttachmentsService attachmentsService, ISubjectsService subjectsService, ITermsService termsService,
			IExamsService examsService, IMapper mapper)
		{
			_questionsService = questionsService;
			_attachmentsService = attachmentsService;
			_recruitsService = recruitsService;
			_subjectsService = subjectsService;
			_termsService = termsService;
			_examsService = examsService;

			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int mode, int recruit)
		{
			var model = new RQIndexViewModel();

			RQMode selectMode = mode.ToRQModeType();
			if (selectMode == RQMode.Unknown)
			{
				//初次載入頁面
				
				model.LoadModeOptions();

				var recruits = await _recruitsService.FetchAsync();
				model.LoadYearOptions(recruits);

				var subitems = recruits.Where(x => x.RecruitEntityType == RecruitEntityType.SubItem);
				model.Subjects = subitems.MapViewModelList(_mapper);


				return Ok(model);
			}

			Recruit selectedRecruit = _recruitsService.GetById(recruit);			

			ValidateRequest(selectedRecruit);
			if (!ModelState.IsValid) return BadRequest(ModelState);


			// subject
			var allSubjects = await _subjectsService.FetchAsync();

			var subject = _recruitsService.FindSubject(selectedRecruit, allSubjects);

			if (subject == null)
			{
				ModelState.AddModelError("subject", "科目不存在");
				return BadRequest(ModelState);
			}

			_subjectsService.LoadSubItems(subject);


			var allTerms = new List<Term>();

			var types = new List<PostType> { PostType.Option, PostType.Resolve };
			var attachments = await _attachmentsService.FetchByTypesAsync(types);
			
			List<Recruit> allRecruits = null;

			var parts = selectedRecruit.SubItems;

			if (selectMode == RQMode.Read)
			{
				if (parts.HasItems())
				{
					foreach (var part in parts)
					{
						var questions = await _questionsService.FetchByRecruitAsync(part, subject);
						var partView = new RQPartViewModel { Points = part.Points };
						partView.Questions = questions.MapViewModelList(_mapper, allRecruits, attachments.ToList(), allTerms);
						model.Parts.Add(partView);
					}

				}
				else
				{
					var questions = await _questionsService.FetchByRecruitAsync(selectedRecruit, subject);

					var partView = new RQPartViewModel { Points = 100 };
					partView.Questions = questions.MapViewModelList(_mapper, allRecruits, attachments.ToList(), allTerms);
					model.Parts.Add(partView);
				}

				return Ok(model);
			}
			else
			{
				if (String.IsNullOrEmpty(CurrentUserId)) return Unauthorized();

				var rootRecruit = await _recruitsService.GetByIdAsync(selectedRecruit.ParentId);

				var exam = new Exam() { ExamType = ExamType.Recruit, RecruitExamType = RecruitExamType.Exactly };
				exam.OptionType = selectedRecruit.OptionType;
				exam.Year = rootRecruit.Year;
				exam.SubjectId = selectedRecruit.SubjectId;

				if (parts.HasItems())
				{
					foreach (var part in parts)
					{
						var questions = (await _questionsService.FetchByRecruitAsync(part, subject)).ToList();
						var examPart = new ExamPart() 
						{ 
							Points = part.Points, OptionCount = part.OptionCount, MultiAnswers = part.MultiAnswers 
						};
						for (int i = 0; i < questions.Count; i++)
						{
							var examQuestion = questions[i].ConversionToExamQuestion(examPart.OptionCount);
							
							examQuestion.Order = i + 1;
							examPart.Questions.Add(examQuestion);
						}

						exam.Parts.Add(examPart);
					}
					
				}
				else
				{
					var questions = (await _questionsService.FetchByRecruitAsync(selectedRecruit, subject)).ToList();

					var examPart = new ExamPart() 
					{ 
						Points = selectedRecruit.Points, OptionCount = selectedRecruit.OptionCount, MultiAnswers = selectedRecruit.MultiAnswers
					};
					for (int i = 0; i < questions.Count; i++)
					{
						var examQuestion = questions[i].ConversionToExamQuestion(examPart.OptionCount);

						examQuestion.Order = i + 1;
						examPart.Questions.Add(examQuestion);
					}

					exam.Parts.Add(examPart);
				}

				await _examsService.CreateAsync(exam, CurrentUserId);

				return Ok(exam.MapViewModel(_mapper, attachments.ToList()));

			}
			

		}

		void ValidateRequest(Recruit selectedRecruit)
		{
			if (selectedRecruit == null)
			{
				ModelState.AddModelError("recruit", "年度不存在");
				return;
			}

			if (selectedRecruit.RecruitEntityType != RecruitEntityType.SubItem)
			{
				ModelState.AddModelError("recruit", "年度錯誤");
				return;
			}

		}

	}
}
