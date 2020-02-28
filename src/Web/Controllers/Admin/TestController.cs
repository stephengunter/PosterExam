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
using ApplicationCore.DataAccess;
using AutoMapper;
using ApplicationCore.ViewServices;
using Web.Models;
using ApplicationCore.Specifications;
using Web.Helpers;

namespace Web.Controllers.Admin
{
	public class ATestController : BaseController
	{

		private DefaultContext _defaultContext;

		public ATestController(DefaultContext defaultContext)
		{
			_defaultContext = defaultContext;
		}


		[HttpGet("")]
		public async Task<ActionResult> Index()
		{
			
			return Ok();
		}

	}
}
