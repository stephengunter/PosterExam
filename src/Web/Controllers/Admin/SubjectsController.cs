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
	public class SubjectsController : BaseAdminController
	{
		private readonly IMapper _mapper;
		private readonly ISubjectsService _subjectsService;

		public SubjectsController(ISubjectsService subjectsService, IMapper mapper)
		{
			_mapper = mapper;
			_subjectsService = subjectsService;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index()
		{
			var subjects = await _subjectsService.FetchAsync();
			subjects = subjects.GetOrdered();
			return Ok(subjects.MapViewModelList(_mapper));
		}

		[HttpGet("create")]
		public ActionResult Create() => Ok(new SubjectViewModel());
		

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] SubjectViewModel model)
		{
			await ValidateRequestAsync(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var subject = model.MapEntity(_mapper);
			subject.SetCreated(CurrentUserId);

			subject = await _subjectsService.CreateAsync(subject);

			return Ok(subject);
		}

		[HttpGet("edit/{id}")]
		public async Task<ActionResult> Edit(int id)
		{
			var subject = await _subjectsService.GetByIdAsync(id);
			if (subject == null) return NotFound();

			var model = subject.MapViewModel(_mapper);
			return Ok(model);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> Update(int id, [FromBody] SubjectViewModel model)
		{
			var subject = await _subjectsService.GetByIdAsync(id);
			if (subject == null) return NotFound();

			await ValidateRequestAsync(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			subject = model.MapEntity(_mapper, subject);
			subject.SetUpdated(CurrentUserId);

			await _subjectsService.UpdateAsync(subject);

			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			var subject = await _subjectsService.GetByIdAsync(id);
			if (subject == null) return NotFound();

			subject.SetUpdated(CurrentUserId);
			await _subjectsService.RemoveAsync(subject);

			return Ok();
		}

		async Task ValidateRequestAsync(SubjectViewModel model)
		{
			
			if (model.ParentId > 0)
			{
				var parent = await _subjectsService.GetByIdAsync(model.ParentId);
				if(parent == null) ModelState.AddModelError("parentId", "主科目不存在");
			}
		}


	}
}
