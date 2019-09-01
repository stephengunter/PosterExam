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
	public class TermsController : BaseAdminController
	{
		private readonly IMapper _mapper;
		private readonly ITermsService _termsService;

		public TermsController(ITermsService termsService, IMapper mapper)
		{
			_mapper = mapper;
			_termsService = termsService;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int subject, int parent = 0)
		{
			var terms = await _termsService.FetchAsync(subject, parent);
			return Ok(terms.MapViewModelList(_mapper));
		}

		[HttpGet("create")]
		public async Task<ActionResult> Create(int subject, int parent)
		{
			int maxOrder = await _termsService.GetMaxOrderAsync(subject, parent);
			var model = new TermViewModel()
			{
				Order = maxOrder + 1,
				SubjectId = subject,
				ParentId = parent
			};
			return Ok(model);
		}

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] TermViewModel model)
		{
			await ValidateRequestAsync(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var term = model.MapEntity(_mapper);
			term.Text = term.Text.ReplaceNewLine();
			term.SetCreated(CurrentUserId);

			term = await _termsService.CreateAsync(term);

			return Ok(term);
		}

		[HttpGet("edit/{id}")]
		public async Task<ActionResult> Edit(int id)
		{
			var term = await _termsService.GetByIdAsync(id);
			if (term == null) return NotFound();

			var model = term.MapViewModel(_mapper);
			model.Text = model.Text.ReplaceBrToNewLine();
			return Ok(model);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> Update(int id, [FromBody] TermViewModel model)
		{
			var term = await _termsService.GetByIdAsync(id);
			if (term == null) return NotFound();
			
			await ValidateRequestAsync(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			term = model.MapEntity(_mapper, term);
			term.Text = term.Text.ReplaceNewLine();
			term.SetUpdated(CurrentUserId);

			await _termsService.UpdateAsync(term);

			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			var term = await _termsService.GetByIdAsync(id);
			if (term == null) return NotFound();

			term.SetUpdated(CurrentUserId);
			await _termsService.RemoveAsync(term);

			return Ok();
		}

		async Task ValidateRequestAsync(TermViewModel model)
		{
			
			if (model.ParentId > 0)
			{
				var parent = await _termsService.GetByIdAsync(model.ParentId);
				if(parent == null) ModelState.AddModelError("parentId", "主科目不存在");
			}
		}


	}
}
