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
using ApplicationCore.Logging;

namespace Web.Controllers.Admin
{
	public class ATestController : ControllerBase
	{

		private readonly IQuestionsService _questionsService;
		private readonly IMapper _mapper;

		public ATestController(IQuestionsService questionsService, IMapper mapper)
		{
			_questionsService = questionsService;
			_mapper = mapper;
		}


		[HttpGet("")]
		public async Task<ActionResult> Index()
		{
			var question = _questionsService.GetById(24);

		
			question.Options = question.Options.ToList().Shuffle(4);
			

			return Ok(question.MapViewModel(_mapper));
		}


	}
}
