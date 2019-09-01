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
	public class QuestionsController : BaseAdminController
	{
		private readonly IMapper _mapper;
		private readonly IQuestionsService _questionsService;
		private readonly ITermsService _termsService;

		public QuestionsController(IQuestionsService questionsService, ITermsService termsService, IMapper mapper)
		{
			_mapper = mapper;
			_questionsService = questionsService;
			_termsService = termsService;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int subject)
		{
			var questions = await _questionsService.FetchAsync(subject);
			return Ok(questions.MapViewModelList(_mapper));
		}

		[HttpGet("create")]
		public async Task<ActionResult> Create(int subject, int term = 0)
		{
			var model = new QuestionViewModel { SubjectId = subject, TermId = term };
			if (term > 0)
			{
				var selectedTerm = await _termsService.GetByIdAsync(term);
				if (selectedTerm == null)
				{
					ModelState.AddModelError("termId", "科目不存在");
					return BadRequest(ModelState);
				}
				await _termsService.LoadParentIdsAsync(selectedTerm);
				model.Term = selectedTerm.MapViewModel(_mapper);
			}

			
			return Ok(model);
		}

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] QuestionViewModel model)
		{
			await ValidateRequestAsync(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var question = model.MapEntity(_mapper);
			question.SetCreated(CurrentUserId);

			question = await _questionsService.CreateAsync(question);

			return Ok(question);
		}

		[HttpGet("edit/{id}")]
		public async Task<ActionResult> Edit(int id)
		{
			var question = await _questionsService.GetByIdAsync(id);
			if (question == null) return NotFound();

			var model = question.MapViewModel(_mapper);
			return Ok(model);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> Update(int id, [FromBody] QuestionViewModel model)
		{
			var question = await _questionsService.GetByIdAsync(id);
			if (question == null) return NotFound();

			await ValidateRequestAsync(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			question = model.MapEntity(_mapper, question);
			question.SetUpdated(CurrentUserId);

			await _questionsService.UpdateAsync(question);

			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			var question = await _questionsService.GetByIdAsync(id);
			if (question == null) return NotFound();

			question.SetUpdated(CurrentUserId);
			await _questionsService.RemoveAsync(question);

			return Ok();
		}

		async Task ValidateRequestAsync(QuestionViewModel model)
		{
			var subject = await _questionsService.GetByIdAsync(model.SubjectId);
			if (subject == null) ModelState.AddModelError("subjectId", "科目不存在");
		}


	}
}
