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
	public class TestController : BaseAdminController
	{

		private readonly IMapper _mapper;
		private readonly ITermsService _termsService;
		private readonly ISubjectsService _subjectsService;
		

		public TestController(ITermsService termsService, ISubjectsService subjectsService, IMapper mapper)
		{
			_mapper = mapper;
			_termsService = termsService;
			_subjectsService = subjectsService;
		}

		[HttpGet("")]
		public ActionResult Index(int term)
		{
			Term selectedTerm = _termsService.GetById(term);
			if (selectedTerm == null)
			{
				ModelState.AddModelError("term", "條文不存在");
				return BadRequest(ModelState);
			}
			var test = selectedTerm.GetSubIds();

			var model = selectedTerm.MapViewModel(_mapper);
			return Ok(model);

		}


	}
}
