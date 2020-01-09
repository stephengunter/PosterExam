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
	public class ATestController : ControllerBase
	{

		private readonly IQuestionsService _questionsService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly IRecruitsService _recruitsService;
		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly IMapper _mapper;

		public ATestController(IQuestionsService questionsService, IRecruitsService recruitsService, IAttachmentsService attachmentsService,
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
		public async Task<ActionResult> Index(int id)
		{
			var question = _questionsService.GetById(id);
			if (question == null) return NotFound();

			var allRecruits = await _recruitsService.GetAllAsync();
			//選項的附圖
			var attachments = await _attachmentsService.FetchAsync(PostType.Option);

			var model = question.MapViewModel(_mapper, allRecruits.ToList(), attachments.ToList());

			return Ok(model);
		}


	}
}
