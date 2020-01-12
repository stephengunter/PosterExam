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
		private readonly IReviewRecordsService _reviewRecordsService;
		private readonly IMapper _mapper;

		public ResolvesController(IResolvesService resolvesService, IReviewRecordsService reviewRecordsService, IMapper mapper)
		{
			_resolvesService = resolvesService;
			_reviewRecordsService = reviewRecordsService;
			_mapper = mapper;
		}

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] ResolveViewModel model)
		{
			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var resolve = model.MapEntity(_mapper, CurrentUserId);

			resolve.Reviewed = true;
			resolve = await _resolvesService.CreateAsync(resolve);

			var reviewRecord = new ReviewRecord { Reviewed = true, Type = ReviewableType.Resolve, PostId = resolve.Id };
			reviewRecord.SetCreated(CurrentUserId);
			await _reviewRecordsService.CreateAsync(reviewRecord);


			return Ok(resolve.Id);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> Update(int id, [FromBody] ResolveViewModel model)
		{
			var existingEntity = await _resolvesService.GetByIdAsync(id);
			if (existingEntity == null) return NotFound();

			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var resolve = model.MapEntity(_mapper, CurrentUserId);

			await _resolvesService.UpdateAsync(existingEntity, resolve);

			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			var resolve = await _resolvesService.GetByIdAsync(id);
			if (resolve == null) return NotFound();

			resolve.SetUpdated(CurrentUserId);
			await _resolvesService.RemoveAsync(resolve);

			return Ok();
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
