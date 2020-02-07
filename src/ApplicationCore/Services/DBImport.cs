using ApplicationCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Models;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ApplicationCore.Services
{
    public interface IDBImportService
    {
		void ImportQuestions(DefaultContext _context, List<Question> models);
		void ImportSubjects(DefaultContext _context, List<Subject> models);
		void ImportTerms(DefaultContext _context, List<Term> models);
		void ImportOptions(DefaultContext _context, List<Option> models);
		void ImportTermQuestions(DefaultContext _context, List<TermQuestion> models);
		void ImportResolves(DefaultContext _context, List<Resolve> models);
		void ImportRecruitQuestions(DefaultContext _context, List<RecruitQuestion> models);
		void ImportNotes(DefaultContext _context, List<Note> models);
		void ImportUploadFiles(DefaultContext _context, List<UploadFile> models);
		void ImportReviewRecords(DefaultContext _context, List<ReviewRecord> models);
	}

    public class DBImportService : IDBImportService
    {
		public void ImportTerms(DefaultContext _context, List<Term> models)
		{
			var connectionString = _context.Database.GetDbConnection().ConnectionString;

			var newTerms = new List<Term>();
			foreach (var termModel in models)
			{
				var existingEntity = _context.Terms.Find(termModel.Id);
				if (existingEntity == null) newTerms.Add(termModel);
				else Update(_context, existingEntity, termModel);
			}

			_context.SaveChanges();

			using (var context = new DefaultContext(connectionString))
			{
				context.Terms.AddRange(newTerms);

				context.Database.OpenConnection();
				try
				{
					context.Database.ExecuteSqlCommand($"SET IDENTITY_INSERT Terms ON");
					context.SaveChanges();
					context.Database.ExecuteSqlCommand($"SET IDENTITY_INSERT Terms OFF");
				}
				finally
				{
					context.Database.CloseConnection();
				}
			}

		}
		public void ImportSubjects(DefaultContext _context, List<Subject> models)
		{
			var connectionString = _context.Database.GetDbConnection().ConnectionString;

			var newSubjects = new List<Subject>();
			foreach (var subjectModel in models)
			{
				var existingEntity = _context.Subjects.Find(subjectModel.Id);
				if (existingEntity == null) newSubjects.Add(subjectModel);
				else Update(_context, existingEntity, subjectModel);
			}

			_context.SaveChanges();

			using (var context = new DefaultContext(connectionString))
			{
				context.Subjects.AddRange(newSubjects);

				context.Database.OpenConnection();
				try
				{
					context.Database.ExecuteSqlCommand($"SET IDENTITY_INSERT Subjects ON");
					context.SaveChanges();
					context.Database.ExecuteSqlCommand($"SET IDENTITY_INSERT Subjects OFF");
				}
				finally
				{
					context.Database.CloseConnection();
				}
			}

		}

		public void ImportQuestions(DefaultContext _context, List<Question> models)
        {
			var connectionString = _context.Database.GetDbConnection().ConnectionString;

			var newQuestions = new List<Question>();
			foreach (var questionModel in models)
			{
				var existingEntity = _context.Questions.Find(questionModel.Id);
				if (existingEntity == null) newQuestions.Add(questionModel);
				else Update(_context, existingEntity, questionModel);
			}

			_context.SaveChanges();

			using (var context = new DefaultContext(connectionString))
			{
				context.Questions.AddRange(newQuestions);

				context.Database.OpenConnection();
				try
				{
					context.Database.ExecuteSqlCommand($"SET IDENTITY_INSERT Questions ON");
					context.SaveChanges();
					context.Database.ExecuteSqlCommand($"SET IDENTITY_INSERT Questions OFF");
				}
				finally
				{
					context.Database.CloseConnection();
				}
			}

		}

		public void ImportOptions(DefaultContext _context, List<Option> models)
		{
			var connectionString = _context.Database.GetDbConnection().ConnectionString;

			var newOptions = new List<Option>();
			foreach (var optionModel in models)
			{
				var existingEntity = _context.Options.Find(optionModel.Id);
				if (existingEntity == null) newOptions.Add(optionModel);
				else Update(_context, existingEntity, optionModel);
			}

			_context.SaveChanges();

			using (var context = new DefaultContext(connectionString))
			{
				context.Options.AddRange(newOptions);

				context.Database.OpenConnection();
				try
				{
					context.Database.ExecuteSqlCommand($"SET IDENTITY_INSERT Options ON");
					context.SaveChanges();
					context.Database.ExecuteSqlCommand($"SET IDENTITY_INSERT Options OFF");
				}
				finally
				{
					context.Database.CloseConnection();
				}
			}

		}

		public void ImportTermQuestions(DefaultContext _context, List<TermQuestion> models)
		{
			var connectionString = _context.Database.GetDbConnection().ConnectionString;

			var newTermQuestions = new List<TermQuestion>();
			foreach (var termQuestionModel in models)
			{
				var existingEntity = _context.TermQuestions.FirstOrDefault(x => x.TermId == termQuestionModel.TermId && x.QuestionId == termQuestionModel.QuestionId);
				if (existingEntity == null) newTermQuestions.Add(termQuestionModel);
				else
				{
					var entry = _context.Entry(existingEntity);
					entry.CurrentValues.SetValues(termQuestionModel);
					entry.State = EntityState.Modified;
				}
			}
			_context.TermQuestions.AddRange(newTermQuestions);
			_context.SaveChanges();

		}

		public void ImportResolves(DefaultContext _context, List<Resolve> models)
		{
			var connectionString = _context.Database.GetDbConnection().ConnectionString;

			var newResolves = new List<Resolve>();
			foreach (var resolveModel in models)
			{
				var existingEntity = _context.Resolves.Find(resolveModel.Id);
				if (existingEntity == null) newResolves.Add(resolveModel);
				else Update(_context, existingEntity, resolveModel);
			}

			_context.SaveChanges();

			using (var context = new DefaultContext(connectionString))
			{
				context.Resolves.AddRange(newResolves);

				context.Database.OpenConnection();
				try
				{
					context.Database.ExecuteSqlCommand($"SET IDENTITY_INSERT Resolves ON");
					context.SaveChanges();
					context.Database.ExecuteSqlCommand($"SET IDENTITY_INSERT Resolves OFF");
				}
				finally
				{
					context.Database.CloseConnection();
				}
			}

		}

		public void ImportRecruitQuestions(DefaultContext _context, List<RecruitQuestion> models)
		{
			var connectionString = _context.Database.GetDbConnection().ConnectionString;

			var newRecruitQuestions = new List<RecruitQuestion>();
			foreach (var recruitQuestionModel in models)
			{
				var existingEntity = _context.RecruitQuestions.FirstOrDefault(x => x.RecruitId == recruitQuestionModel.RecruitId && x.QuestionId == recruitQuestionModel.QuestionId);
				if (existingEntity == null) newRecruitQuestions.Add(recruitQuestionModel);
				else
				{
					var entry = _context.Entry(existingEntity);
					entry.CurrentValues.SetValues(recruitQuestionModel);
					entry.State = EntityState.Modified;
				}
			}
			_context.RecruitQuestions.AddRange(newRecruitQuestions);
			_context.SaveChanges();

		}

		public void ImportNotes(DefaultContext _context, List<Note> models)
		{
			var connectionString = _context.Database.GetDbConnection().ConnectionString;

			var newNotes = new List<Note>();
			foreach (var noteModel in models)
			{
				if (noteModel == null)
				{
					var test = "";
				}
				var existingEntity = _context.Notes.Find(noteModel.Id);
				if (existingEntity == null) newNotes.Add(noteModel);
				else Update(_context, existingEntity, noteModel);
			}

			_context.SaveChanges();

			using (var context = new DefaultContext(connectionString))
			{
				context.Notes.AddRange(newNotes);

				context.Database.OpenConnection();
				try
				{
					context.Database.ExecuteSqlCommand($"SET IDENTITY_INSERT Notes ON");
					context.SaveChanges();
					context.Database.ExecuteSqlCommand($"SET IDENTITY_INSERT Notes OFF");
				}
				finally
				{
					context.Database.CloseConnection();
				}
			}

		}

		public void ImportUploadFiles(DefaultContext _context, List<UploadFile> models)
		{
			var connectionString = _context.Database.GetDbConnection().ConnectionString;

			var newUploadFiles = new List<UploadFile>();
			foreach (var uploadFileModel in models)
			{
				var existingEntity = _context.UploadFiles.Find(uploadFileModel.Id);
				if (existingEntity == null) newUploadFiles.Add(uploadFileModel);
				else Update(_context, existingEntity, uploadFileModel);
			}

			_context.SaveChanges();

			using (var context = new DefaultContext(connectionString))
			{
				context.UploadFiles.AddRange(newUploadFiles);

				context.Database.OpenConnection();
				try
				{
					context.Database.ExecuteSqlCommand($"SET IDENTITY_INSERT UploadFiles ON");
					context.SaveChanges();
					context.Database.ExecuteSqlCommand($"SET IDENTITY_INSERT UploadFiles OFF");
				}
				finally
				{
					context.Database.CloseConnection();
				}
			}

		}

		public void ImportReviewRecords(DefaultContext _context, List<ReviewRecord> models)
		{
			var connectionString = _context.Database.GetDbConnection().ConnectionString;

			var newReviewRecords = new List<ReviewRecord>();
			foreach (var reviewRecordModel in models)
			{
				var existingEntity = _context.ReviewRecords.Find(reviewRecordModel.Id);
				if (existingEntity == null) newReviewRecords.Add(reviewRecordModel);
				else Update(_context, existingEntity, reviewRecordModel);
			}

			_context.SaveChanges();

			using (var context = new DefaultContext(connectionString))
			{
				context.ReviewRecords.AddRange(newReviewRecords);

				context.Database.OpenConnection();
				try
				{
					context.Database.ExecuteSqlCommand($"SET IDENTITY_INSERT ReviewRecords ON");
					context.SaveChanges();
					context.Database.ExecuteSqlCommand($"SET IDENTITY_INSERT ReviewRecords OFF");
				}
				finally
				{
					context.Database.CloseConnection();
				}
			}

		}

		void Update(DefaultContext _context, BaseEntity existingEntity, BaseEntity model)
		{
			var entry = _context.Entry(existingEntity);
			entry.CurrentValues.SetValues(model);
			entry.State = EntityState.Modified;
		}

	}
}
