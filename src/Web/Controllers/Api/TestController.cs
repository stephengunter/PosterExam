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
using AutoMapper;
using ApplicationCore.Helpers;
using ApplicationCore.ViewServices;

namespace Web.Controllers.Api
{
	public class ATestController : BaseApiController
	{

		private readonly IUsersService _usersService;
		private readonly IAuthService _authService;

		public ATestController(IUsersService usersService, IAuthService authService)
		{
			_usersService = usersService;
			_authService = authService;
		}

		[HttpGet]
		public async Task<ActionResult> Index()
		{
			var test = new Bill();
			return Ok(test.Code);
		}

		[HttpGet("ex")]
		public async Task<ActionResult> Ex()
		{
			throw new ExamNotRecruitQuestionSelected();
		}


	}

}