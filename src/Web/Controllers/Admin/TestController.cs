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
using Newtonsoft.Json;

namespace Web.Controllers.Admin
{
	public class ATestController : BaseController
	{
		private readonly ITermsService _termsService;
		private readonly IMapper _mapper;
		public ATestController(ITermsService termsService, IMapper mapper)
		{
			_termsService = termsService;
			_mapper = mapper;
		}


		[HttpGet("")]
		public async Task<ActionResult> Index()
		{
			return Ok();
		}

	}
}
