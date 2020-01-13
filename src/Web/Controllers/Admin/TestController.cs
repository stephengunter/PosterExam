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

		private readonly IResolvesService _resolvesService;
		private readonly IReviewRecordsService _reviewRecordsService;
		private readonly IMapper _mapper;

		public ATestController(IResolvesService resolvesService, IReviewRecordsService reviewRecordsService, IMapper mapper)
		{
			_resolvesService = resolvesService;
			_reviewRecordsService = reviewRecordsService;
			_mapper = mapper;
		}


		[HttpGet("")]
		public async Task<ActionResult> Index()
		{
			
			var resolves = await _resolvesService.FetchAsync();
			foreach (var entity in resolves)
			{
				//entity.Text = entity.Text.ReplaceNewLine();


				entity.Text = entity.Text.Replace("<br>", "<br/>");
				await _resolvesService.UpdateAsync(entity);
			}

			return Ok();
		}


	}
}
