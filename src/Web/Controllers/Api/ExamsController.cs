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
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Exceptions;
using Web.Models;
using Web.Helpers;

namespace Web.Controllers
{
	[Authorize]
	public class ExamsController : BaseController
	{
		private readonly UserManager<User> _userManager;
		private readonly IExamsService _examsService;
		private readonly ISubjectsService _subjectsService;
		private readonly IMapper _mapper;

		public ExamsController(UserManager<User> userManager, IExamsService examsService, ISubjectsService subjectsService,
			IMapper mapper)
		{
			_userManager = userManager;
			_examsService = examsService;
			_subjectsService = subjectsService;
			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int subject = 0, int status = -1, int page = 1, int pageSize = 10)
		{
			var user = await _userManager.FindByIdAsync(CurrentUserId);
			if (user == null) throw new UserNotFoundException(CurrentUserId);
			

			var model = new ExamIndexViewModel();

			if (page < 1) //初次載入頁面
			{
				model.LoadStatusOptions();

				//考試科目
				var rootSubjects = _subjectsService.FetchRootItems();
				model.LoadSubjectOptions(rootSubjects, "全部");

				page = 1;
			}

			var exams = await _examsService.FetchAsync(user);

			if (exams.HasItems())
			{
				exams = exams.FilterByStatus(status.ToExamStaus());

				var subjects = await _subjectsService.FetchAsync();

				if (subject > 0) exams = FilterBySubject(exams, subjects, subject);


				foreach (var exam in exams) exam.LoadSubject(subjects);

				exams = exams.GetOrdered();
			}
			
			model.PagedList = exams.GetPagedList(_mapper, page, pageSize);

			return Ok(model);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> Save(int id, [FromBody] ExamViewModel model)
		{
			var existingEntity = await _examsService.GetByIdAsync(id);
			if (existingEntity == null) return NotFound();

			var exam = model.MapEntity(_mapper, CurrentUserId);

			ValidateSaveRequest(exam);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			_examsService.SaveExam(existingEntity, exam);

			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			var exam = await _examsService.GetByIdAsync(id);
			if (exam == null) throw new EntityNotFoundException(new Exam { Id = id });

			ValidateDeleteRequest(exam);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			await _examsService.DeleteAsync(exam);

			return Ok();
		}


		IEnumerable<Exam> FilterBySubject(IEnumerable<Exam> exams, IEnumerable<Subject> subjects, int subjectId)
		{
			var selectedSubject = subjects.FirstOrDefault(x => x.Id == subjectId);
			if (selectedSubject == null) throw new EntityNotFoundException(new Subject { Id = subjectId });

			return exams.Where(x => x.SubjectId == subjectId);
		}

		

		void ValidateSaveRequest(Exam exam)
		{
			if (exam.UserId != CurrentUserId) ModelState.AddModelError("userId", "權限不足");
		}

		void ValidateDeleteRequest(Exam exam)
		{
			if (!exam.CanDelete) ModelState.AddModelError("exam", "此測驗無法刪除");

			if (exam.UserId != CurrentUserId) ModelState.AddModelError("userId", "權限不足");
		}
	}
}
