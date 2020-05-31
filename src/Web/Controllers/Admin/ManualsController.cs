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
using Web.Models;
using ApplicationCore.Specifications;
using Web.Helpers;
using Web.Controllers;

namespace Web.Controllers.Admin
{
	public class ManualsController : BaseAdminController
	{
		private readonly IManualsService _manualsService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly IMapper _mapper;

		public ManualsController(IManualsService manualsService, IAttachmentsService attachmentsService,
			 IMapper mapper)
		{
			_manualsService = manualsService;
			_attachmentsService = attachmentsService;
			_mapper = mapper;
		}

		

		[HttpGet("")]
		public async Task<ActionResult> Index(int active = 1)
		{
			var manuals = await _manualsService.FetchAsync(active.ToBoolean());

			return Ok(manuals.MapViewModelList(_mapper));
		}

		[HttpGet("create")]
		public ActionResult Create()
		{
			return Ok(new ManualViewModel());
		}

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] ManualViewModel model)
		{
			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			model.Order = model.Active ? 0 : -1;

			var manual = model.MapEntity(_mapper, CurrentUserId);

			manual = await _manualsService.CreateAsync(manual);

			return Ok(manual.Id);
		}

		void ValidateRequest(ManualViewModel model)
		{
			if (String.IsNullOrEmpty(model.Title)) ModelState.AddModelError("title", "請填寫標題");
			if (String.IsNullOrEmpty(model.Content)) ModelState.AddModelError("content", "請填寫內容");
		}
	}
}
