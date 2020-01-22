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
using Web.Helpers.ViewServices;

namespace Web.Controllers
{
	[Authorize]
	public class ExamsController : BaseController
	{
		private readonly UserManager<User> _userManager;

		private readonly IQuestionsService _questionsService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly IRecruitsService _recruitsService;
		private readonly IExamsService _examsService;
		private readonly ISubjectsService _subjectsService;
		private readonly IMapper _mapper;

		public ExamsController(UserManager<User> userManager, IQuestionsService questionsService,
			IAttachmentsService attachmentsService, IRecruitsService recruitsService, 
			IExamsService examsService, ISubjectsService subjectsService,
			IMapper mapper)
		{
			_userManager = userManager;

			_questionsService = questionsService;
			_recruitsService = recruitsService;
			_attachmentsService = attachmentsService;
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
				model.LoadExamTypeOptions();
				model.LoadRecruitExamTypeOptions();

				var yearRecruits = await _recruitsService.FetchByTypeAsync(RecruitEntityType.Year);
				model.LoadYearOptions(yearRecruits);

				model.LoadStatusOptions();

				//考試科目
				var examSubjects = await _subjectsService.FetchExamSubjectsAsync();
				model.LoadSubjectOptions(examSubjects, "全部");

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

		[HttpGet("create")]
		public async Task<ActionResult> Create(int recruit = 0, int type = -1, int rtype = -1, int subject = 0, int year = 0)
		{
			Exam exam = null;

			if (recruit > 0)   //歷屆試題模式, 完全相同
			{
				var selectedRecruit = _recruitsService.GetById(recruit);
				if (selectedRecruit == null) throw new EntityNotFoundException(new Recruit { Id = recruit });

				if (selectedRecruit.RecruitEntityType != RecruitEntityType.SubItem)
				{
					ModelState.AddModelError("recruit", "年度錯誤");
					return BadRequest(ModelState);
				}

				exam = await InitByRecruitAsync(selectedRecruit);

			}
			else 
			{
				var selectedSubject = _subjectsService.GetById(subject);
				if (selectedSubject == null) throw new EntityNotFoundException(new Subject { Id = subject });

				ExamType selectType = type.ToExamType();
				if (selectType == ExamType.Unknown)
				{
					ModelState.AddModelError("type", "題庫來源錯誤");
					return BadRequest(ModelState);
				}

				selectType = ExamType.Recruit; //目前僅支援"歷屆試題"


				if (selectType == ExamType.Recruit) //歷屆試題
				{
					RecruitExamType selectRType = rtype.ToRecruitExamType();
					if (selectRType == RecruitExamType.Unknown)
					{
						ModelState.AddModelError("rtype", "試題配置錯誤");
						return BadRequest(ModelState);
					}

					selectRType = RecruitExamType.Exactly; //目前僅支援"完全相同"
					if (selectRType == RecruitExamType.Exactly)
					{
						if (year < 1)
						{
							var yearRecruits = await _recruitsService.FetchByTypeAsync(RecruitEntityType.Year);
							year = yearRecruits.FirstOrDefault().Year;
						}
						

						var selectedRecruit = await _recruitsService.FindByYearSubjectAsync(year, selectedSubject);
						if (selectedRecruit == null)
						{
							ModelState.AddModelError("recruit", "年度錯誤");
							return BadRequest(ModelState);
						}

						exam = await InitByRecruitAsync(selectedRecruit);
					}

				}

				

				//var allSubjects = await _subjectsService.FetchAsync();

				//Subject selectedSubject = null;
				//Recruit selectedRecruit = null;

			}

			await _examsService.CreateAsync(exam, CurrentUserId);

			var types = new List<PostType> { PostType.Option, PostType.Resolve };
			var attachments = await _attachmentsService.FetchByTypesAsync(types);

			return Ok(exam.MapViewModel(_mapper, attachments.ToList()));
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

		[HttpGet("edit/{id}")]
		public async Task<ActionResult> Edit(int id)
		{
			var exam = _examsService.GetById(id);
			var alloptions = await _questionsService.FetchAllOptionsAsync();

			var examQuestions = exam.Parts.SelectMany(p => p.Questions);
			foreach (var item in examQuestions) item.LoadOptions();

			var types = new List<PostType> { PostType.Option, PostType.Resolve };
			var attachments = await _attachmentsService.FetchByTypesAsync(types);

			return Ok(exam.MapViewModel(_mapper, attachments.ToList()));
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

		#region InitExam

		async Task<Exam> InitByRecruitAsync(Recruit selectedRecruit)
		{
			var allSubjects = await _subjectsService.FetchAsync();

			var selectedSubject = _recruitsService.FindSubject(selectedRecruit, allSubjects);
			_subjectsService.LoadSubItems(selectedSubject);

			var rootRecruit = await _recruitsService.GetByIdAsync(selectedRecruit.ParentId);
			var exam = new Exam()
			{
				ExamType = ExamType.Recruit,
				RecruitExamType = RecruitExamType.Exactly,
				OptionType = selectedRecruit.OptionType,
				Year = rootRecruit.Year,
				SubjectId = selectedRecruit.SubjectId
			};

			var parts = selectedRecruit.SubItems;

			if (parts.HasItems())
			{
				foreach (var part in parts)
				{
					var questions = (await _questionsService.FetchByRecruitAsync(part, selectedSubject)).ToList();
					var examPart = new ExamPart()
					{
						Points = part.Points,
						OptionCount = part.OptionCount,
						MultiAnswers = part.MultiAnswers
					};
					for (int i = 0; i < questions.Count; i++)
					{
						var examQuestion = questions[i].ConversionToExamQuestion(examPart.OptionCount);

						examQuestion.Order = i + 1;
						examPart.Questions.Add(examQuestion);
					}

					exam.Parts.Add(examPart);
				}

			}
			else
			{
				var questions = (await _questionsService.FetchByRecruitAsync(selectedRecruit, selectedSubject)).ToList();

				var examPart = new ExamPart()
				{
					Points = selectedRecruit.Points,
					OptionCount = selectedRecruit.OptionCount,
					MultiAnswers = selectedRecruit.MultiAnswers
				};
				for (int i = 0; i < questions.Count; i++)
				{
					var examQuestion = questions[i].ConversionToExamQuestion(examPart.OptionCount);

					examQuestion.Order = i + 1;
					examPart.Questions.Add(examQuestion);
				}

				exam.Parts.Add(examPart);
			}

			return exam;
		}

		#endregion

		#region Helper

		IEnumerable<Exam> FilterBySubject(IEnumerable<Exam> exams, IEnumerable<Subject> subjects, int subjectId)
		{
			var selectedSubject = subjects.FirstOrDefault(x => x.Id == subjectId);
			if (selectedSubject == null) throw new EntityNotFoundException(new Subject { Id = subjectId });

			return exams.Where(x => x.SubjectId == subjectId);
		}

		#endregion


		#region Validate

		void ValidateSaveRequest(Exam exam)
		{
			if (exam.UserId != CurrentUserId) ModelState.AddModelError("userId", "權限不足");
		}

		void ValidateDeleteRequest(Exam exam)
		{
			if (!exam.CanDelete) ModelState.AddModelError("exam", "此測驗無法刪除");

			if (exam.UserId != CurrentUserId) ModelState.AddModelError("userId", "權限不足");
		}

		void ValidateEditRequest(Exam exam)
		{
			if (exam.UserId != CurrentUserId) ModelState.AddModelError("userId", "權限不足");

			if (exam.IsComplete) ModelState.AddModelError("isComplete", "此測驗已經完成");

		}

		#endregion
	}
}
