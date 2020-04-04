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
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using ApplicationCore.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Data.SqlClient;
using Infrastructure.Entities;
using System.Data;
using Microsoft.AspNetCore.Http;

namespace Web.Controllers.Admin
{
	public class DBController : BaseAdminController
	{
		private readonly AdminSettings _adminSettings;
		private readonly DefaultContext _context;
		private readonly IDBImportService _dBImportService;

		public DBController(IOptions<AdminSettings> adminSettings, DefaultContext context, IDBImportService dBImportService)
		{
			_adminSettings = adminSettings.Value;
			_context = context;
			_dBImportService = dBImportService;
		}

		async Task<string> ReadFileTextAsync(IFormFile file)
		{
			var result = new StringBuilder();
			using (var reader = new StreamReader(file.OpenReadStream()))
			{
				while (reader.Peek() >= 0) result.AppendLine(await reader.ReadLineAsync());
			}
			return result.ToString();

		}



		string GetDbName(string connectionString)
		{
			
			var builder = new SqlConnectionStringBuilder(connectionString);
			return builder.InitialCatalog;
		}

		[HttpGet("dbname")]
		public ActionResult DBName()
		{
			var connectionString = _context.Database.GetDbConnection().ConnectionString;
			string dbName = GetDbName(connectionString);


			return Ok(dbName);
		}

		[HttpPost("migrate")]
		public ActionResult Migrate(AdminRequest model)
		{
			
			ValidateRequest(model, _adminSettings);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			_context.Database.Migrate();

			return Ok();
		}

		[HttpPost("backup")]
		public ActionResult Backup(AdminRequest model)
		{
			ValidateRequest(model, _adminSettings);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var connectionString = _context.Database.GetDbConnection().ConnectionString;
			string dbName = GetDbName(connectionString);

			var folderPath = BackupFolder();
			var fileName = Path.Combine(folderPath, $"{dbName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.bak");

			string cmdText = $"BACKUP DATABASE [{dbName}] TO DISK = '{fileName}'";
			using (var conn = new SqlConnection(connectionString))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand(cmdText, conn))
				{
					int result = cmd.ExecuteNonQuery();

				}
				conn.Close();
			}

			return Ok();
		}

		[HttpPost("export")]
		public ActionResult Export(AdminRequest model)
		{
			ValidateRequest(model, _adminSettings);
			if (!ModelState.IsValid) return BadRequest(ModelState);
			
			var folderPath = BackupFolder();
		
			_context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
			
			var subjects = _context.Subjects.ToList();
			SaveJson(folderPath, new Subject().GetType().Name, JsonConvert.SerializeObject(subjects));
			
			var terms = _context.Terms.ToList();
			SaveJson(folderPath, new Term().GetType().Name, JsonConvert.SerializeObject(terms));

			var questions = _context.Questions.ToList();
			SaveJson(folderPath, new Question().GetType().Name, JsonConvert.SerializeObject(questions));

			var options = _context.Options.ToList();
			SaveJson(folderPath, new Option().GetType().Name, JsonConvert.SerializeObject(options));

			var termQuestions = _context.TermQuestions.ToList();
			SaveJson(folderPath, new TermQuestion().GetType().Name, JsonConvert.SerializeObject(termQuestions));


			var resolves = _context.Resolves.ToList();
			SaveJson(folderPath, new Resolve().GetType().Name, JsonConvert.SerializeObject(resolves));

			var recruits = _context.Recruits.ToList();
			SaveJson(folderPath, new Recruit().GetType().Name, JsonConvert.SerializeObject(recruits));

			var recruitQuestions = _context.RecruitQuestions.ToList();
			SaveJson(folderPath, new RecruitQuestion().GetType().Name, JsonConvert.SerializeObject(recruitQuestions));

			var notes = _context.Notes.ToList();
			SaveJson(folderPath, new Note().GetType().Name, JsonConvert.SerializeObject(notes));

			var uploads = _context.UploadFiles.ToList();
			SaveJson(folderPath, new UploadFile().GetType().Name, JsonConvert.SerializeObject(uploads));

			var reviewRecords = _context.ReviewRecords.ToList();
			SaveJson(folderPath, new ReviewRecord().GetType().Name, JsonConvert.SerializeObject(reviewRecords));

			return Ok();
		}

		[HttpPost("import")]
		public async Task<IActionResult> Import([FromForm] AdminRequest model)
		{
			ValidateRequest(model, _adminSettings);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			if (model.Files.Count < 1)
			{
				ModelState.AddModelError("files", "必須上傳檔案");
				return BadRequest(ModelState);
			}

			var extensions = model.Files.Select(item => Path.GetExtension(item.FileName).ToLower());
			if (extensions.Any(x => x != ".json"))
			{
				ModelState.AddModelError("files", "檔案格式錯誤");
				return BadRequest(ModelState);
			}

			string content = "";
		
			var file = model.GetFile(new Subject().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var subjectModels = JsonConvert.DeserializeObject<List<Subject>>(content);
				_dBImportService.ImportSubjects(_context, subjectModels);
			}

			file = model.GetFile(new Term().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var termModels = JsonConvert.DeserializeObject<List<Term>>(content);
				_dBImportService.ImportTerms(_context, termModels);
			}

			

			file = model.GetFile(new Question().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var questionModels = JsonConvert.DeserializeObject<List<Question>>(content);
				_dBImportService.ImportQuestions(_context, questionModels);
			}

			file = model.GetFile(new Option().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var optionModels = JsonConvert.DeserializeObject<List<Option>>(content);
				_dBImportService.ImportOptions(_context, optionModels);
			}

			file = model.GetFile(new TermQuestion().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var termQuestionModels = JsonConvert.DeserializeObject<List<TermQuestion>>(content);
				_dBImportService.ImportTermQuestions(_context, termQuestionModels);
			}

			file = model.GetFile(new Resolve().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var resolveModels = JsonConvert.DeserializeObject<List<Resolve>>(content);
				_dBImportService.ImportResolves(_context, resolveModels);
			}

			file = model.GetFile(new Recruit().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var recruitModels = JsonConvert.DeserializeObject<List<Recruit>>(content);
				_dBImportService.ImportRecruits(_context, recruitModels);
			}

			file = model.GetFile(new RecruitQuestion().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var recruitQuestionModels = JsonConvert.DeserializeObject<List<RecruitQuestion>>(content);
				_dBImportService.ImportRecruitQuestions(_context, recruitQuestionModels);
			}

			file = model.GetFile(new Note().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var noteModels = JsonConvert.DeserializeObject<List<Note>>(content);
				_dBImportService.ImportNotes(_context, noteModels);
			}

			file = model.GetFile(new UploadFile().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var uploadFileModels = JsonConvert.DeserializeObject<List<UploadFile>>(content);
				_dBImportService.ImportUploadFiles(_context, uploadFileModels);
			}

			file = model.GetFile(new ReviewRecord().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var reviewRecordModels = JsonConvert.DeserializeObject<List<ReviewRecord>>(content);
				_dBImportService.ImportReviewRecords(_context, reviewRecordModels);
			}

			return Ok();
		}

		void SaveJson(string folderPath, string name, string content)
		{
			var filePath = Path.Combine(folderPath, $"{name}.json");
			System.IO.File.WriteAllText(filePath, content);
		}

		string BackupFolder()
		{
			var path = Path.Combine(_adminSettings.BackupPath, DateTime.Today.ToDateNumber().ToString());
			if (!Directory.Exists(path)) Directory.CreateDirectory(path);

			return path;
		}

	}
}
