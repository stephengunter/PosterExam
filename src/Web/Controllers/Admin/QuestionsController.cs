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
		private readonly IAttachmentsService _attachmentsService;
		private readonly IRecruitsService _recruitsService;
		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly IMapper _mapper;

		public QuestionsController(IQuestionsService questionsService, IRecruitsService recruitsService, IAttachmentsService attachmentsService,
			ISubjectsService subjectsService, ITermsService termsService, IMapper mapper)
		{
			_questionsService = questionsService;
			_recruitsService = recruitsService;
			_attachmentsService = attachmentsService;
			_subjectsService = subjectsService;
			_termsService = termsService;

			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int subject, int term, string recruits = "", int page = 1, int pageSize = 10)
		{
			Subject selectedSubject = _subjectsService.GetById(subject);
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

			var recruitIds = recruits.SplitToIds();

			var questions = await _questionsService.FetchAsync(selectedSubject, termIds, recruitIds);

			var allRecruits = await _recruitsService.GetAllAsync();

			//選項的附圖
			var attachments = await _attachmentsService.FetchAsync(PostType.Option);

			var pageList = questions.GetPagedList(_mapper, allRecruits.ToList(), attachments.ToList(), page, pageSize);

			foreach (var item in pageList.ViewList)
			{
				item.Options = item.Options.OrderByDescending(o => o.Correct).ToList();
			}

			return Ok(pageList);
		}

		[HttpGet("create")]
		public ActionResult Create()
		{
			return Ok(new QuestionViewModel());
		}

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] QuestionViewModel model)
		{
			await ValidateRequestAsync(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var question = model.MapEntity(_mapper, CurrentUserId);

			question = await _questionsService.CreateAsync(question);

			foreach (var option in question.Options)
			{
				foreach (var attachment in option.Attachments)
				{
					attachment.PostType = PostType.Option;
					attachment.PostId = option.Id;
					attachment.SetCreated(CurrentUserId);
					await _attachmentsService.CreateAsync(attachment);
				}
			}

			var options = question.Options;
			var view = question.MapViewModel(_mapper);

			return Ok(view);
		}

		[HttpGet("edit/{id}")]
		public async Task<ActionResult> Edit(int id)
		{
			var question = _questionsService.GetById(id);
			if (question == null) return NotFound();

			var allRecruits = await _recruitsService.GetAllAsync();
			//選項的附圖
			var attachments = await _attachmentsService.FetchAsync(PostType.Option);

			var model = question.MapViewModel(_mapper, allRecruits.ToList(), attachments.ToList());
			
			return Ok(model);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> Update(int id, [FromBody] QuestionViewModel model)
		{
			var existingEntity = _questionsService.GetById(id);
			if (existingEntity == null) return NotFound();

			await ValidateRequestAsync(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var question = model.MapEntity(_mapper, CurrentUserId);

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

			if (model.Options.HasItems())
			{
				var correctOptions = model.Options.Where(item => item.Correct).ToList();
				if (correctOptions.IsNullOrEmpty()) ModelState.AddModelError("options", "必須要有正確的選項");
				else if (correctOptions.Count > 1)
				{
					if (!model.MultiAnswers) ModelState.AddModelError("options", "單選題只能有一個正確選項");

				}
			}
									
		}


	}
}
