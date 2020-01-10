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
		public async Task<ActionResult> Index(int recruit)
		{
			Recruit selectedRecruit = _recruitsService.GetById(recruit);
			
			
			return Ok(selectedRecruit.MapViewModel(_mapper));
		}


	}
}
