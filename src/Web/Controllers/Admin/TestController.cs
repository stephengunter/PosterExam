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
using ApplicationCore.Logging;

namespace Web.Controllers.Admin
{
	public class ATestController : ControllerBase
	{

		private readonly IQuestionsService _questionsService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly IRecruitsService _recruitsService;
		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly IMapper _mapper;

		private readonly ILogger _logger;

		public ATestController(IQuestionsService questionsService, IRecruitsService recruitsService, IAttachmentsService attachmentsService,
			ISubjectsService subjectsService, ITermsService termsService, IMapper mapper, ILogger logger)
		{
			_questionsService = questionsService;
			_recruitsService = recruitsService;
			_attachmentsService = attachmentsService;
			_subjectsService = subjectsService;
			_termsService = termsService;

			_mapper = mapper;
			_logger = logger;
		}
		[HttpGet("")]
		public async Task<ActionResult> Index(int recruit = 0)
		{
			Subject selectedSubject = _subjectsService.GetById(1);
			ICollection<int> termIds = null;

			//var recruitIds = _recruitsService.GetSubIds(recruit);
			//if (recruitIds.HasItems()) recruitIds.Add(recruit);

			var allRecruits = await _recruitsService.GetAllAsync();

			Recruit selectedRecruit = null;
			List<int> recruitIds = new List<int>();
			if (recruit > 0)
			{
			
				selectedRecruit = allRecruits.FirstOrDefault(x => x.Id == recruit); // await _recruitsService.GetByIdAsync(recruit);
			
				if (selectedRecruit == null)
				{
					ModelState.AddModelError("recruit", "招考年度不存在");
					return BadRequest(ModelState);
				}

				recruitIds.Add(recruit);

				if (selectedRecruit.RecruitEntityType == RecruitEntityType.SubItem)
				{
					var partIds = allRecruits.Where(x => x.ParentId == recruit).Select(part => part.Id);
					recruitIds.AddRange(partIds.ToList());
					recruitIds.Add(recruit);
				}
			}


			var questions = await _questionsService.FetchAsync(selectedSubject, termIds, recruitIds);

			

			//選項的附圖
			var attachments = await _attachmentsService.FetchAsync(PostType.Option);

			//不載入條文
			List<Term> allTerms = null;

			var pageList = questions.GetPagedList(_mapper, allRecruits.ToList(), attachments.ToList(), allTerms);

			foreach (var item in pageList.ViewList)
			{
				item.Options = item.Options.OrderByDescending(o => o.Correct).ToList();
			}

			return Ok(pageList);
		}


	}
}
