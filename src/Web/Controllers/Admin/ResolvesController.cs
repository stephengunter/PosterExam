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
	public class ResolvesController : BaseAdminController
	{
		private readonly IResolvesService _resolvesService;
		private readonly IMapper _mapper;

		public ResolvesController(IResolvesService resolvesService, IMapper mapper)
		{
			_resolvesService = resolvesService;
			_mapper = mapper;
		}

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] ResolveViewModel model)
		{
			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var resolve = model.MapEntity(_mapper, CurrentUserId);

			resolve = await _resolvesService.CreateAsync(resolve);
			
			return Ok(resolve.Id);
		}


		void ValidateRequest(ResolveViewModel model)
		{
			if (String.IsNullOrEmpty(model.Text))
			{
				ModelState.AddModelError("text", "必須填寫內容");
				return;
			}
		}

	}
}
