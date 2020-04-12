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
using ApplicationCore.Exceptions;
using System.IO;

namespace Web.Controllers.Admin
{
	public class ATestController : BaseController
	{
		private readonly IDataService _dataService;
		private readonly IQuestionsService _questionsService;
		private readonly IRecruitsService _recruitsService;
		private readonly ISubjectsService _subjectsService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly IExamsService _examsService;
		private readonly IMapper _mapper;
		private readonly ApplicationCore.Logging.ILogger _logger;

		private readonly AdminSettings _adminSettings;

		public ATestController(IOptions<AdminSettings> adminSettings, IDataService dataService, IQuestionsService questionsService, 
			IRecruitsService recruitsService, ISubjectsService subjectsService,
			IAttachmentsService attachmentsService, IExamsService examsService, IMapper mapper,
			ApplicationCore.Logging.ILogger logger)
		{
			_mapper = mapper;
			_questionsService = questionsService;
			_examsService = examsService;
			_dataService = dataService;
			_recruitsService = recruitsService;
			_subjectsService = subjectsService;
			_attachmentsService = attachmentsService;
			_logger = logger;

			_adminSettings = adminSettings.Value;
		}

		private readonly string _folderPath;
		private readonly string _filePath;



		private readonly DefaultContext _defaultContext;
		//public ATestController(DefaultContext defaultContext)
		//{
		//	_defaultContext = defaultContext;
		//}

		
		[HttpGet("")]
		public async Task<ActionResult> Index()
		{
			SetExamRecruitId();
			return Ok();
		}

		[HttpGet("ex")]
		public async Task<ActionResult> Ex()
		{
			throw new ExamNotRecruitQuestionSelected();
		}



		void SetExamRecruitId()
		{
			var exams = _defaultContext.Exams.ToList();
			foreach (var exam in exams)
			{
				if (exam.Year > 0)
				{
					int year = exam.Year;
					var recruit = _defaultContext.Recruits.Where(x => x.Year == year).FirstOrDefault();
					exam.RecruitId = recruit.Id;
				}
			}

			_defaultContext.SaveChanges();
		}

	}
}
