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
using Newtonsoft.Json;

namespace Web.Controllers.Admin
{
	public class ResolvesController : BaseAdminController
	{
		private readonly IResolvesService _resolvesService;
		private readonly IReviewRecordsService _reviewRecordsService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly IMapper _mapper;

		public ResolvesController(IResolvesService resolvesService, IReviewRecordsService reviewRecordsService,
			IAttachmentsService attachmentsService,IMapper mapper)
		{
			_resolvesService = resolvesService;
			_reviewRecordsService = reviewRecordsService;
			_attachmentsService = attachmentsService;
			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int question = 0, int page = 1, int pageSize = 10)
		{
			var resolves = await _resolvesService.FetchAsync(question);

			List<UploadFile> attachments = null;

			if (resolves.IsNullOrEmpty()) return Ok(resolves.GetPagedList(_mapper, attachments, page, pageSize));

			var postIds = resolves.Select(x => x.Id).ToList();

			attachments = (await _attachmentsService.FetchAsync(PostType.Resolve, postIds)).ToList();


			var pageList = resolves.GetPagedList(_mapper, attachments.ToList(), page, pageSize);
			return Ok(pageList);
		}

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] ResolveViewModel model)
		{
			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var resolve = model.MapEntity(_mapper, CurrentUserId);

			resolve.Reviewed = true;
			resolve = await _resolvesService.CreateAsync(resolve);

			if (model.Attachments.HasItems())
			{
				var attachments = model.Attachments.Select(item => item.MapEntity(_mapper, CurrentUserId)).ToList();
				foreach (var attachment in attachments)
				{
					attachment.PostType = PostType.Resolve;
					attachment.PostId = resolve.Id;
				}

				_attachmentsService.CreateMany(attachments);

				resolve.Attachments = attachments;
			}

			var reviewRecord = new ReviewRecord { Reviewed = true, Type = ReviewableType.Resolve, PostId = resolve.Id };
			reviewRecord.SetCreated(CurrentUserId);
			await _reviewRecordsService.CreateAsync(reviewRecord);
			

			return Ok(resolve.MapViewModel(_mapper));
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> Update(int id, [FromBody] ResolveViewModel model)
		{
			var existingEntity = await _resolvesService.GetByIdAsync(id);
			if (existingEntity == null) return NotFound();

			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var resolve = model.MapEntity(_mapper, CurrentUserId);
			resolve.Reviewed = true;

			await _resolvesService.UpdateAsync(existingEntity, resolve);


			if (model.Attachments.HasItems())
			{
				var attachments = model.Attachments.Select(item => item.MapEntity(_mapper, CurrentUserId)).ToList();
				foreach (var attachment in attachments)
				{
					attachment.PostType = PostType.Resolve;
					attachment.PostId = resolve.Id;
				}

				await _attachmentsService.SyncAttachmentsAsync(resolve, attachments);

				resolve.Attachments = attachments;
			}
			else
			{
				await _attachmentsService.SyncAttachmentsAsync(resolve, null);
			}

			var reviewRecord = new ReviewRecord { Reviewed = true, Type = ReviewableType.Resolve, PostId = resolve.Id };
			reviewRecord.SetCreated(CurrentUserId);
			await _reviewRecordsService.CreateAsync(reviewRecord);

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
			if (String.IsNullOrEmpty(model.Text) && model.Attachments.IsNullOrEmpty())
			{
				ModelState.AddModelError("text", "必須填寫內容");
				return;
			}
		}

	}
}
