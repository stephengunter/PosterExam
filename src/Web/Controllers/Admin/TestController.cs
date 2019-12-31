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

		private readonly IRecruitsService _recruitsService;
		private readonly IMapper _mapper;

		public TestController(IRecruitsService recruitsService, IMapper mapper)

		{
			_recruitsService = recruitsService;
			_mapper = mapper;
		}


		[HttpGet("")]
		public async Task<ActionResult> Index()
		{
			var model = new RecruitViewModel { Title = "jiji", Year = 105 };
			model.SubItems = new List<RecruitViewModel>()
			{
				new RecruitViewModel { Title = "qq" , SubjectId = 1},
				new RecruitViewModel { Title = "pl" , SubjectId = 2}
			};

			var recruit = model.MapEntity(_mapper, "");

			recruit = await _recruitsService.CreateAsync(recruit, recruit.SubItems);

			return Ok(recruit.Id);
		}

		

	}
}
