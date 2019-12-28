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

		private readonly IMapper _mapper;
		private readonly IQuestionsService _questionsService;

		public TestController(IQuestionsService questionsService, IMapper mapper)
		{
			_mapper = mapper;
			_questionsService = questionsService;
		}
		
		[HttpGet("")]
		public ActionResult Index(string recruits = "")
		{

			var recruitIds = recruits.SplitToIds();
			return Ok(recruitIds);



		}


	}
}
