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

		TestBaseOption<int> myTest;


		[HttpGet("")]
		public async Task<ActionResult> Index()
		{
			this.myTest = new TestBaseOption<int>(30, "jiiji");
			return Ok(myTest);
		}

		class TestBaseOption<TKey>
		{
			public TestBaseOption(TKey value, string text)
			{
				this.Value = value;
				this.Text = text;
			}
			public TKey Value { get; set; }
			public string Text { get; set; }

		}

	}
}
