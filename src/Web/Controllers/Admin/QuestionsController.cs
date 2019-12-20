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
		private readonly IQuestionsService _questionsService;
		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly IMapper _mapper;

		public QuestionsController(IQuestionsService questionsService, ISubjectsService subjectsService, ITermsService termsService,
			 IMapper mapper)
		{
			_questionsService = questionsService;
			_subjectsService = subjectsService;
			_termsService = termsService;

			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int subject, int term, string keyword = "", int page = 1, int pageSize = 10)
		{
			Subject selectedSubject = await _subjectsService.GetByIdAsync(subject);
			if (selectedSubject == null)
			{
				ModelState.AddModelError("subject", "科目不存在");
				return BadRequest(ModelState);
			}

			Term selectedTerm = null;
			ICollection<int> termIds = null;
			if (term > 0)
			{
				selectedTerm =_termsService.GetById(term);
				if (selectedTerm == null)
				{
					ModelState.AddModelError("term", "條文不存在");
					return BadRequest(ModelState);
				}

				termIds = selectedTerm.GetSubIds();
				termIds.Add(selectedTerm.Id);
			}

			var questions = await _questionsService.FetchAsync(selectedSubject, termIds);

			var pageList = questions.GetPagedList(_mapper, page, pageSize);

			foreach (var item in pageList.ViewList)
			{
				item.Options = item.Options.OrderByDescending(o => o.Correct).ToList();
			}

			return Ok(pageList);
		}

		[HttpGet("create")]
		public async Task<ActionResult> Create(int subject, int term = 0)
		{
			var model = new QuestionViewModel { SubjectId = subject, TermId = term };

			if (term > 0) model.Term = await LoadTermViewModelAsync(term);
			
			return Ok(model);
		}

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] QuestionViewModel model)
		{
			await ValidateRequestAsync(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var question = model.MapEntity(_mapper);

			var recruitQuestions = question.RecruitQuestions;
			if (recruitQuestions.IsNullOrEmpty())
			{

			}
			question.SetCreated(CurrentUserId);

			question = await _questionsService.CreateAsync(question);

			return Ok(question);
		}

		async Task<TermViewModel> LoadTermViewModelAsync(int termId)
		{
			var term = await _termsService.GetByIdAsync(termId);
			if (term == null) return null;

			await _termsService.LoadParentIdsAsync(term);
			return term.MapViewModel(_mapper);
		}

		[HttpGet("edit/{id}")]
		public async Task<ActionResult> Edit(int id)
		{
			var question = _questionsService.GetById(id);
			if (question == null) return NotFound();

			var model = question.MapViewModel(_mapper);

			if (question.TermId > 0) model.Term = await LoadTermViewModelAsync(question.TermId);
			
			return Ok(model);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> Update(int id, [FromBody] QuestionViewModel model)
		{
			var existingEntity = _questionsService.GetById(id);
			if (existingEntity == null) return NotFound();

			await ValidateRequestAsync(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var question = model.MapEntity(_mapper);
			question.SetUpdated(CurrentUserId);

			await _questionsService.UpdateAsync(existingEntity, question);

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
			var subject = await _subjectsService.GetByIdAsync(model.SubjectId);
			if (subject == null) ModelState.AddModelError("subjectId", "科目不存在");

			if (model.Options.IsNullOrEmpty())
			{
				ModelState.AddModelError("options", "必須要有選項");
			}
			else
			{
				var correctOptions = model.Options.Where(item => item.Correct).ToList();
				if (correctOptions.IsNullOrEmpty()) ModelState.AddModelError("options", "必須要有一個正確選項");
				else if (correctOptions.Count > 1) ModelState.AddModelError("options", "只能有一個正確選項");
			}
									
		}


	}
}
