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

		private readonly IMapper _mapper;
		private readonly ITermsService _termsService;
		private readonly ISubjectsService _subjectsService;

		public ATestController(ITermsService termsService, ISubjectsService subjectsService, IMapper mapper)
		{
			_mapper = mapper;
			_termsService = termsService;
			_subjectsService = subjectsService;
		}


		[HttpGet("")]
		public async Task<ActionResult> Index(int subject, int parent = -1, string keyword = "", bool subItems = true)
		{
			Subject selectedSubject = await _subjectsService.GetByIdAsync(subject);
			if (selectedSubject == null)
			{
				ModelState.AddModelError("subject", "科目不存在");
				return BadRequest(ModelState);
			}

			var terms = await _termsService.FetchAsync(selectedSubject, parent);
			if (terms.HasItems())
			{
				if (subItems) _termsService.LoadSubItems(terms);

				var keywords = keyword.GetKeywords();
				if (keywords.HasItems()) terms = terms.FilterByKeyword(keywords);

				terms = terms.GetOrdered();
			}


			return Ok(terms.MapViewModelList(_mapper));
		}

		

	}
}
