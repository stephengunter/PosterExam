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
using Infrastructure.Views;

namespace Web.Controllers.Admin
{
	public class RecruitsController : BaseAdminController
	{
		private readonly IMapper _mapper;
		private readonly IRecruitsService _recruitsService;
		private readonly ISubjectsService _subjectsService;

		public RecruitsController(IRecruitsService recruitsService, ISubjectsService subjectsService, IMapper mapper)
		{
			_mapper = mapper;
			_recruitsService = recruitsService;
			_subjectsService = subjectsService;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int parent = 0, bool active = true, int year = 0)
		{
			var recruits = await _recruitsService.FetchAsync(parent, active);
			if (year > 0) recruits = recruits.Where(x => x.Year == year);

			recruits = recruits.GetOrdered();

			if (parent == 0)
			{
				_recruitsService.LoadSubItems(recruits);

				var subjects = await _subjectsService.FetchAsync();
				_subjectsService.LoadSubItems(subjects);

				foreach (var recruit in recruits)
				{
					foreach (var subItem in recruit.SubItems)
					{
						if (subItem.SubjectId > 0)
						{
							var subject = subjects.FirstOrDefault(x => x.Id == subItem.SubjectId);
							subject.GetSubIds();

							subItem.Subject = subject;
							
							var subjectIds = new List<int> { subject.Id };
							subjectIds.AddRange(subject.SubIds);
							subItem.SubjectIds = subjectIds;

						}
					}
				}
			}

			return Ok(recruits.MapViewModelList(_mapper));
		}

		[HttpGet("create")]
		public ActionResult Create()
		{
			var model = new RecruitEditForm();

			var subjects = _subjectsService.FetchRootItems();
			model.SubjectOptions = subjects.Select(item => new BaseOption<int>(item.Id, item.Title)).ToList();

			return Ok(model);
		}
		
		

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] RecruitViewModel model)
		{
			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			model.Order = model.Active ? 0 : -1;

			var recruit = model.MapEntity(_mapper, CurrentUserId);

			recruit = await _recruitsService.CreateAsync(recruit, recruit.SubItems);

			return Ok(recruit.Id);
		}

		[HttpGet("edit/{id}")]
		public async Task<ActionResult> Edit(int id)
		{
			var recruit = await _recruitsService.GetByIdAsync(id);
			if (recruit == null) return NotFound();

			_recruitsService.LoadSubItems(recruit);
			var model = new RecruitEditForm() {  Recruit = recruit.MapViewModel(_mapper) };

			var subjects = _subjectsService.FetchRootItems();
			model.SubjectOptions = subjects.Select(item => new BaseOption<int>(item.Id, item.Title)).ToList();

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

			await _recruitsService.UpdateAsync(existingEntity, recruit, recruit.SubItems);

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

			if (model.SubItems.IsNullOrEmpty()) ModelState.AddModelError("subItems", "必須要有筆試項目");

			var subjectIds = model.SubItems.Select(x => x.SubjectId).Distinct();
			if (subjectIds.Count() != model.SubItems.Count())
			{
				ModelState.AddModelError("subItems", "筆試科目重複了");
			}


		}

	}
}
