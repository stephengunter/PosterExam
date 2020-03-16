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

namespace Web.Controllers.Admin
{
	public class NotesController : BaseAdminController
	{
		private readonly INotesService _notesService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly IMapper _mapper;

		public NotesController(INotesService notesService, ISubjectsService subjectsService, ITermsService termsService,
			 IAttachmentsService attachmentsService, IMapper mapper)
		{
			_notesService = notesService;
			_subjectsService = subjectsService;
			_termsService = termsService;
			_attachmentsService = attachmentsService;

			_mapper = mapper;
		}
		
		[HttpGet("")]
		public async Task<ActionResult> Index(int term = 0, int subject = 0, string keyword = "")
		{
			if (term > 0)
			{
				var selectedTerm = _termsService.GetById(term);
				var termViewModel = await LoadTermViewModelAsync(selectedTerm);


				if (termViewModel.SubItems.HasItems()) return Ok(termViewModel.SubItems);
				else return Ok(new List<TermViewModel> { termViewModel });
			}
			else if (subject > 0)
			{
				var keywords = keyword.GetKeywords();
				Subject selectedSubject = await _subjectsService.GetByIdAsync(subject);
				int parent = -1;
				//科目底下所有條文
				var terms = (await _termsService.FetchAsync(selectedSubject, parent)).Where(x => !x.ChapterTitle);
				var termIds = terms.Select(x => x.Id).ToList();

				if (terms.HasItems())
				{
					_termsService.LoadSubItems(terms);

					if (keywords.HasItems()) terms = terms.FilterByKeyword(keywords);
					terms = terms.GetOrdered();
				}

				var termViewModelList = new List<TermViewModel>();
				foreach (var item in terms)
				{
					var termViewModel = await LoadTermViewModelAsync(item);
					termViewModelList.Add(termViewModel);
				}


				if (keywords.HasItems())
				{
					var notes = await _notesService.FetchAsync(termIds);
					notes = notes.FilterByKeyword(keywords);

					if (notes.HasItems())
					{
						foreach (int termId in notes.Select(x => x.TermId).Distinct())
						{
							var exist = termViewModelList.FirstOrDefault(x => x.Id == termId);
							if (exist == null)
							{
								var selectedTerm = _termsService.GetById(termId);
								var noteInTerms = notes.Where(x => x.TermId == termId);

								var termViewModel = await LoadTermViewModelAsync(selectedTerm);
								termViewModelList.Add(termViewModel);
							}
						}

						termViewModelList = termViewModelList.OrderBy(item => item.Order).ToList();

					}


				}


				return Ok(termViewModelList);
			}

			ModelState.AddModelError("params", "錯誤的查詢參數");
			return BadRequest(ModelState);
		}

		[HttpGet("create")]
		public async Task<ActionResult> Create(int term)
		{
			Term selectedTerm = await _termsService.GetByIdAsync(term);
			if (selectedTerm == null)
			{
				ModelState.AddModelError("term", "條文課綱不存在");
				return BadRequest(ModelState);
			}
			int parent = 0;
			int maxOrder = await _notesService.GetMaxOrderAsync(selectedTerm, parent);
			var model = new NoteViewModel()
			{
			
				Order = maxOrder + 1,
				TermId = term,
				ParentId = parent
			};
			return Ok(model);
		}



		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] NoteViewModel model)
		{
			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var note = model.MapEntity(_mapper, CurrentUserId);
			note = await _notesService.CreateAsync(note);

			if (model.Attachments.HasItems())
			{
				var attachments = model.Attachments.Select(item => item.MapEntity(_mapper, CurrentUserId)).ToList();
				foreach (var attachment in attachments)
				{
					attachment.PostType = PostType.Note;
					attachment.PostId = note.Id;
				}

				_attachmentsService.CreateMany(attachments);

				note.Attachments = attachments;
			}


			return Ok(note.MapViewModel(_mapper));
		}

		[HttpGet("{id}")]
		public ActionResult Details(int id)
		{
			var note = _notesService.GetById(id);
			if (note == null) return NotFound();

			return Ok(note.MapViewModel(_mapper));
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> Update(int id, [FromBody] NoteViewModel model)
		{
			var existingEntity = await _notesService.GetByIdAsync(id);
			if (existingEntity == null) return NotFound();

			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var note = model.MapEntity(_mapper, CurrentUserId);

			await _notesService.UpdateAsync(existingEntity, note);


			if (model.Attachments.HasItems())
			{
				var attachments = model.Attachments.Select(item => item.MapEntity(_mapper, CurrentUserId)).ToList();
				foreach (var attachment in attachments)
				{
					attachment.PostType = PostType.Note;
					attachment.PostId = note.Id;
				}

				await _attachmentsService.SyncAttachmentsAsync(note, attachments);

				note.Attachments = attachments;
			}
			else
			{
				await _attachmentsService.SyncAttachmentsAsync(note, null);
			}

			return Ok();
		}

		[HttpPost("order")]
		public async Task<ActionResult> Order([FromBody] OrderRequest model)
		{
			await _notesService.UpdateOrderAsync(model.TargetId, model.ReplaceId, model.Up);
			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			var note = await _notesService.GetByIdAsync(id);
			if (note == null) return NotFound();

			note.SetUpdated(CurrentUserId);
			await _notesService.RemoveAsync(note);

			return Ok();
		}

		async Task<TermViewModel> LoadTermViewModelAsync(Term term, IEnumerable<Note> notes = null)
		{
			if (notes == null)
			{
				var termIds = new List<int>() { term.Id };
				if (term.SubItems.HasItems()) termIds.AddRange(term.GetSubIds());
				notes = await _notesService.FetchAsync(termIds);
			}

			var postIds = notes.Select(x => x.Id).ToList();
			var attachments = (await _attachmentsService.FetchAsync(PostType.Note, postIds)).ToList();

			var noteViewList = notes.MapViewModelList(_mapper, attachments.ToList());

			var termViewModel = term.MapViewModel(_mapper);
			termViewModel.LoadNotes(noteViewList);

			return termViewModel;
		}

		void ValidateRequest(NoteViewModel model)
		{
			if (String.IsNullOrEmpty(model.Text) && model.Attachments.IsNullOrEmpty())
			{
				ModelState.AddModelError("text", "必須填寫內容");
				return;
			}
		}

	}
}
