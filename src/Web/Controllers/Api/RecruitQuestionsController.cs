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
			RQMode selectMode = mode.ToRQModeType();
			if (selectMode == RQMode.Unknown)
			{
				//初次載入頁面
				var rqIndexModel = new RQIndexViewModel();
				rqIndexModel.LoadModeOptions();

				var recruits = await _recruitsService.FetchAsync();
				rqIndexModel.LoadYearOptions(recruits);

				var subitems = recruits.Where(x => x.RecruitEntityType == RecruitEntityType.SubItem);
				rqIndexModel.Subjects = subitems.MapViewModelList(_mapper);

				return Ok(rqIndexModel);
			}


			if (selectMode != RQMode.Read)
			{
				ModelState.AddModelError("RQMode", "僅支援閱讀模式");
				return BadRequest(ModelState);
			}

			var model = new RQViewModel();

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

			model.LoadTitle();

			return Ok(model);
			
			

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
