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

namespace Web.Controllers
{
	[Authorize]
	public class ExamsController : BaseController
	{
		private readonly IQuestionsService _questionsService;
		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly IMapper _mapper;

		public ExamsController(IQuestionsService questionsService, ISubjectsService subjectsService, ITermsService termsService,
			 IMapper mapper)
		{
			_questionsService = questionsService;
			_subjectsService = subjectsService;
			_termsService = termsService;

			_mapper = mapper;
		}

		[HttpGet("")]
		public ActionResult Index(int subject, int term, string recruits = "", int page = 1, int pageSize = 10)
		{
			return Ok();
		}

		[HttpGet("")]
		public ActionResult Create()
		{
			return Ok();
		}
	}
}
