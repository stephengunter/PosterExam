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
			
			if (!String.IsNullOrEmpty(keyword))
			{

				var notes = await GetNotesByKeywordsAsync(subject, keyword);
				var postIds = notes.Select(x => x.Id).ToList();
				var attachments = (await _attachmentsService.FetchAsync(PostType.Note, postIds)).ToList();

				var termIds = notes.Select(x => x.TermId).Distinct();
				var termViewModels = new List<TermViewModel>();
				foreach (var itemId in termIds)
				{
					termViewModels.Add(LoadTermViewModel(itemId, notes, attachments));
				}
				return Ok(termViewModels);
			}

			if (term < 1) //初次載入頁面
			{
				var model = new NoteAdminViewModel();

				int parentSubjectId = 0;
				var rootSubjects = await _subjectsService.FetchAsync(parentSubjectId);
				rootSubjects = rootSubjects.GetOrdered();

				_subjectsService.LoadSubItems(rootSubjects);

				model.RootSubjects = rootSubjects.MapViewModelList(_mapper);

				var subjectSubitems = rootSubjects.SelectMany(p => p.SubItems);
				model.Subjects = subjectSubitems.MapViewModelList(_mapper);

				foreach (var subjectModel in model.Subjects)
				{
					int parentTermId = 0;
					var terms = await _termsService.FetchAsync(subjectSubitems.FirstOrDefault(x => x.Id == subjectModel.Id), parentTermId);
					if (terms.HasItems())
					{
						_termsService.LoadSubItems(terms);
						terms = terms.GetOrdered();
					}

					model.Terms.AddRange(terms.MapViewModelList(_mapper));
				}

				model.Subject = model.RootSubjects.FirstOrDefault();

				return Ok(model);

			}
			else
			{
				var selectedTerm = _termsService.GetById(term);
				var termIds = new List<int> { selectedTerm.Id };
				if (selectedTerm.SubItems.HasItems()) termIds.AddRange(selectedTerm.GetSubIds());

				var notes = await _notesService.FetchAsync(termIds);

				var postIds = notes.Select(x => x.Id).ToList();
				var attachments = (await _attachmentsService.FetchAsync(PostType.Note, postIds)).ToList();

				var noteViewList = notes.MapViewModelList(_mapper, attachments.ToList());

				var termViewModel = selectedTerm.MapViewModel(_mapper);
				termViewModel.LoadNotes(noteViewList);

				

				if(termViewModel.SubItems.HasItems()) return Ok(termViewModel.SubItems);
				else return Ok(new List<TermViewModel> { termViewModel });

			}
			
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

		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			var note = await _notesService.GetByIdAsync(id);
			if (note == null) return NotFound();

			note.SetUpdated(CurrentUserId);
			await _notesService.RemoveAsync(note);

			return Ok();
		}

		async Task<List<Note>> GetNotesByKeywordsAsync(int subject, string keyword)
		{
			var selectedSubject = _subjectsService.GetById(subject);
			var subjectIds = new List<int> { subject };
			if (selectedSubject.SubItems.HasItems()) subjectIds.AddRange(selectedSubject.GetSubIds());

			var terms = await _termsService.FetchAsync(new TermFilterBySubjectsSpecification(subjectIds));
			var termIds = terms.Select(x => x.Id).ToList();

			var notes = await _notesService.FetchAsync(new NoteTermFilterSpecification(termIds));


			var keywords = keyword.GetKeywords();
			if (keywords.HasItems()) notes = notes.FilterByKeyword(keywords);


			return notes.ToList();
		}

		TermViewModel LoadTermViewModel(int term, List<Note> noteList, List<UploadFile> attachments)
		{
			var selectedTerm = _termsService.GetById(term);
			var termIds = new List<int> { selectedTerm.Id };
			if (selectedTerm.SubItems.HasItems()) termIds.AddRange(selectedTerm.GetSubIds());

			var notes = noteList.Where(x => termIds.Contains(x.TermId));

			var noteViewList = notes.MapViewModelList(_mapper, attachments);

			var termViewModel = selectedTerm.MapViewModel(_mapper);
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
