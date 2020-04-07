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
using ApplicationCore.DataAccess;
using AutoMapper;
using ApplicationCore.ViewServices;
using Web.Models;
using ApplicationCore.Specifications;
using Web.Helpers;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using ApplicationCore.Settings;

namespace Web.Controllers.Admin
{
	public class ATestController : BaseController
	{
		private readonly IDataService _dataService;
		private readonly RootSubjectSettings _rootSubjectSettings;
		private readonly IMapper _mapper;

		private readonly ISubjectsService _subjectsService;

		public ATestController(IOptions<RootSubjectSettings> rootSubjectSettings, ISubjectsService subjectsService,
			IDataService dataService, IMapper mapper)
		{
			_dataService = dataService;
			_rootSubjectSettings = rootSubjectSettings.Value;
			_mapper = mapper;

			_subjectsService = subjectsService;
			
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int subject)
		{
			//出題
			var rootSubject = _subjectsService.GetById(subject);
			if (rootSubject == null) return NotFound();
			if (rootSubject.ParentId > 0)
			{
				ModelState.AddModelError("subject", "錯誤的科目");
				return BadRequest(ModelState);
			}


			//取得ExamSettings
			var model = _dataService.FindExamSettings(subject);
			if (model == null) return NotFound();

			//取得每個Subject, Term 底下的QuestionId(精選試題)
			var sqList = _dataService.FindSubjectQuestions(subject);

			var subjectQuestions = sqList.FirstOrDefault(x => x.SubjectId == 17);

			var questionIds = subjectQuestions.GetQuestionIds();

			//var test = view.FirstOrDefault().GetQuestionIds();

			//if (subject == _rootSubjectSettings.FirstId)
			//{
			//	//專業科目(1)：臺灣自然及人文地理
			//	foreach (var partSettings in model.Parts)
			//	{
			//		var subjects = partSettings.Subjects.SelectMany(x => x.)
			//	}
			//}
			//else
			//{
			//	//專業科目(2)：郵政法規大意及交通安全常識
			//}

			return Ok();
		}


		void testSave(int subject)
		{
			//test
			var models = new List<SubjectQuestionsViewModel>()
			{
				new SubjectQuestionsViewModel
				{
					SubjectId = 12,
					TermQuestions = new List<TermQuestionsViewModel>
					{
						new TermQuestionsViewModel
						{
						   TermId = 134,
						   QuestionIds = new int[] { 3, 19, 66 },
						   SubItems = new List<TermQuestionsViewModel>
						   {
								new TermQuestionsViewModel
								{
									TermId = 256,
									QuestionIds = new int[] { 90, 17, 45 }
								},
								new TermQuestionsViewModel
								{
									TermId = 654,
									QuestionIds = new int[] { 9, 117, 545 }
								}
						   }
						},
						new TermQuestionsViewModel
						{
						   TermId = 140,
						   QuestionIds = new int[] { 77, 113 },
						   SubItems = new List<TermQuestionsViewModel>
						   {
								new TermQuestionsViewModel
								{
									TermId = 214,
									QuestionIds = new int[] { 217, 129, 405 }
								},
								new TermQuestionsViewModel
								{
									TermId = 521,
									QuestionIds = new int[] { 27, 316, 218 }
								}
						   }
						}
					}
				},
				new SubjectQuestionsViewModel
				{
					SubjectId = 17,
					TermQuestions = new List<TermQuestionsViewModel>
					{
						new TermQuestionsViewModel
						{
						   TermId = 234,
						   QuestionIds = new int[] { 87, 119, 178 }
						},
						new TermQuestionsViewModel
						{
						   TermId = 2140,
						   QuestionIds = new int[] { 33, 67 }
						}
					}
				},

			};

			_dataService.SaveSubjectQuestions(subject, models);
		}

	}
}
