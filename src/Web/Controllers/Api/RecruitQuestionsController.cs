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

namespace Web.Controllers.Api
{
	public class RecruitQuestionsController : BaseController
	{
		private readonly IMapper _mapper;
		private readonly IRecruitsService _recruitsService;
		private readonly ISubjectsService _subjectsService;

		public RecruitQuestionsController(IRecruitsService recruitsService, ISubjectsService subjectsService, IMapper mapper)
		{
			_mapper = mapper;
			_recruitsService = recruitsService;
			_subjectsService = subjectsService;
		}

		[HttpGet("")]
		public ActionResult Index(int mode)
		{
			if (mode < 0)
			{
				//初次載入頁面
				var model = new RQIndexViewModel();
				model.LoadModeOptions();

				return Ok(model);
			}

			//if (String.IsNullOrEmpty(mode))
			//{
			//	//初次載入頁面
			//	var model = new RQIndexViewModel();
			//}


			return Ok();

		}

	}
}
