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

namespace Web.Controllers.Admin
{
	public class DBController : BaseAdminController
	{
		private readonly AdminSettings _adminSettings;
		private readonly DefaultContext _context;

		public DBController(IOptions<AdminSettings> adminSettings, DefaultContext context)
		{
			_adminSettings = adminSettings.Value;
			_context = context;
		}

		string GetDbName(string connectionString)
		{
			
			var builder = new SqlConnectionStringBuilder(connectionString);
			return builder.InitialCatalog;
		}

		[HttpPost("migrate")]
		public ActionResult Migrate(AdminRequest model)
		{
			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			_context.Database.Migrate();

			return Ok();
		}

		[HttpPost("backup")]
		public ActionResult Backup(AdminRequest model)
		{
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
			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var questions = _context.Questions.ToList();
			var folderPath = BackupFolder();

			SaveJson(folderPath, "questions", JsonConvert.SerializeObject(questions));

			return Ok();
		}

		[HttpPost("import")]
		public async Task<IActionResult> Import([FromForm] AdminRequest model)
		{
			var file = model.Files.FirstOrDefault();
			
			var result = new StringBuilder();
			using (var reader = new StreamReader(file.OpenReadStream()))
			{
				while (reader.Peek() >= 0)
					result.AppendLine(await reader.ReadLineAsync());
			}

			var questions = JsonConvert.DeserializeObject<List<Question>>(result.ToString());

			var questionModel = questions.FirstOrDefault(x => x.Id == 24);
			var existingEntity = _context.Questions.Find(questionModel.Id);

			Update(existingEntity, questionModel);
			_context.SaveChanges();



			//string cmd = canSetId ? "" : $"SET IDENTITY_INSERT {table} ON; ";


			return Ok();
		}


		void Update(Question existingEntity, Question model)
		{
			var entry = _context.Entry(existingEntity);
			entry.CurrentValues.SetValues(model);
			entry.State = EntityState.Modified;
		}



		void ValidateRequest(AdminRequest model)
		{
			if(model.Key != _adminSettings.Key) ModelState.AddModelError("key", "認證錯誤");

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
