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
		private readonly IMapper _mapper;

		public RecruitQuestionsController(IQuestionsService questionsService, IRecruitsService recruitsService,
			IAttachmentsService attachmentsService, ISubjectsService subjectsService, ITermsService termsService, IMapper mapper)
		{
			_questionsService = questionsService;
			_attachmentsService = attachmentsService;
			_recruitsService = recruitsService;
			_subjectsService = subjectsService;
			_termsService = termsService;

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


			var allTerms = await _termsService.FetchAllAsync();

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
					partView.Questions = questions.GetPagedList(_mapper, allRecruits, attachments.ToList(), allTerms.ToList());
					model.Parts.Add(partView);
				}
				
			}
			else
			{
				var questions = await _questionsService.FetchByRecruitAsync(selectedRecruit, subject);

				var partView = new RQPartViewModel { Points = 100 };
				partView.Questions = questions.GetPagedList(_mapper, allRecruits, attachments.ToList(), allTerms.ToList());
				model.Parts.Add(partView);
			}

			

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
