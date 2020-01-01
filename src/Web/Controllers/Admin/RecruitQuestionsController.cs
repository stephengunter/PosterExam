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

namespace Web.Controllers.Admin
{
	public class RecruitQuestionsController : BaseAdminController
	{
		private readonly IQuestionsService _questionsService;
		private readonly IRecruitsService _recruitsService;
		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly IMapper _mapper;

		public RecruitQuestionsController(IQuestionsService questionsService, IRecruitsService recruitsService,
			ISubjectsService subjectsService, ITermsService termsService, IMapper mapper)
		{
			_questionsService = questionsService;
			_recruitsService = recruitsService;
			_subjectsService = subjectsService;
			_termsService = termsService;

			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int recruit)
		{
			Recruit selectedRecruit = await _recruitsService.GetByIdAsync(recruit);
			if (selectedRecruit == null)
			{
				ModelState.AddModelError("recruit", "科目不存在");
				return BadRequest(ModelState);
			}

			if (selectedRecruit.SubjectId == 0) return Ok(new List<Question>().GetPagedList(_mapper));

			Subject selectedSubject = _subjectsService.GetById(selectedRecruit.SubjectId);
			if (selectedSubject == null)
			{
				ModelState.AddModelError("subject", "科目不存在");
				return BadRequest(ModelState);
			}

			var questions = await _questionsService.FetchByRecruitAsync(selectedRecruit, selectedSubject);
			if (questions.HasItems())
			{
				var allTerms = await _termsService.FetchAllAsync();
				foreach (var question in questions)
				{
					question.LoadTerms(allTerms);
					question.Options = question.Options.OrderByDescending(o => o.Correct).ToList();
				}
			}

			var pageList = questions.GetPagedList(_mapper);

			return Ok(pageList);
		}

	}
}
