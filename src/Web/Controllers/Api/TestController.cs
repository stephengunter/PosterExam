using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using Microsoft.AspNetCore.Identity;
using ApplicationCore.Models;
using ApplicationCore.Auth;
using Microsoft.Extensions.Options;
using ApplicationCore.Services;
using Web.Controllers;
using Microsoft.AspNetCore.Hosting;
using ApplicationCore.Exceptions;

namespace Web.Controllers.Api
{
	public class ATestController : BaseApiController
	{
		
		private readonly IWebHostEnvironment _env;
		public ATestController(IWebHostEnvironment env)
		{
			_env = env;
		}


		[HttpGet]
		public async Task<ActionResult> Index()
		{

			return Ok(_env.WebRootPath);

		}

		[HttpGet("ex")]
		public async Task<ActionResult> Ex()
		{
			throw new ExamNotRecruitQuestionSelected();
		}


	}

}