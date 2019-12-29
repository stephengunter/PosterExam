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
	public class TestController : ControllerBase
	{

		private readonly IQuestionsService _questionsService;
		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly IMapper _mapper;

		public TestController(IQuestionsService questionsService, ISubjectsService subjectsService, ITermsService termsService,
			 IMapper mapper)
		{
			_questionsService = questionsService;
			_subjectsService = subjectsService;
			_termsService = termsService;

			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int subject, string terms = "", string recruits = "", int page = 1, int pageSize = 10)
		{
			Subject selectedSubject = await _subjectsService.GetByIdAsync(subject);
			if (selectedSubject == null)
			{
				ModelState.AddModelError("subject", "科目不存在");
				return BadRequest(ModelState);
			}

			var termIds = terms.SplitToIds();

			var recruitIds = recruits.SplitToIds();

			var questions = await _questionsService.FetchAsync(selectedSubject, termIds, recruitIds);

			var pageList = questions.GetPagedList(_mapper, page, pageSize);

			foreach (var item in pageList.ViewList)
			{
				item.Options = item.Options.OrderByDescending(o => o.Correct).ToList();
			}

			return Ok(pageList);
		}

	}
}
