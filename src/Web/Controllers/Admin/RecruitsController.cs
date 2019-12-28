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
	public class RecruitsController : BaseAdminController
	{
		private readonly IMapper _mapper;
		private readonly IRecruitsService _recruitsService;

		public RecruitsController(IRecruitsService recruitsService, IMapper mapper)
		{
			_mapper = mapper;
			_recruitsService = recruitsService;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(bool active = true, int year = 0)
		{
			
			var recruits = await _recruitsService.FetchAsync(active);
			if (year > 0) recruits = recruits.Where(x => x.Year == year);

			recruits = recruits.GetOrdered();
			return Ok(recruits.MapViewModelList(_mapper));
		}

		[HttpGet("create")]
		public ActionResult Create() => Ok(new RecruitViewModel());
		

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] RecruitViewModel model)
		{
			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			model.Order = model.Active ? 0 : -1;

			var recruit = model.MapEntity(_mapper, CurrentUserId);

			recruit = await _recruitsService.CreateAsync(recruit);

			return Ok(recruit.Id);
		}

		[HttpGet("edit/{id}")]
		public async Task<ActionResult> Edit(int id)
		{
			var recruit = await _recruitsService.GetByIdAsync(id);
			if (recruit == null) return NotFound();

			var model = recruit.MapViewModel(_mapper);
			return Ok(model);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> Update(int id, [FromBody] RecruitViewModel model)
		{
			var existingEntity = _recruitsService.GetById(id);
			if (existingEntity == null) return NotFound();

			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			if (model.Active)
			{
				if (existingEntity.Active == false) model.Order = 0;
			}
			else
			{
				if (existingEntity.Active) model.Order = -1;
			}

			var recruit = model.MapEntity(_mapper, CurrentUserId);

			await _recruitsService.UpdateAsync(existingEntity, recruit);

			return Ok();
		}

		[HttpPost("order")]
		public async Task<ActionResult> Order([FromBody] OrderRequest model)
		{
			await _recruitsService.UpdateOrderAsync(model.TargetId, model.ReplaceId, model.Up);
			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			var recruit = await _recruitsService.GetByIdAsync(id);
			if (recruit == null) return NotFound();

			recruit.SetUpdated(CurrentUserId);
			await _recruitsService.RemoveAsync(recruit);

			return Ok();
		}

		void ValidateRequest(RecruitViewModel model)
		{
			if (String.IsNullOrEmpty(model.Title)) ModelState.AddModelError("subjectId", "請填寫標題");

			if (model.Year <= 0) ModelState.AddModelError("year", "請填寫年度");
			
		}

	}
}
