using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ApplicationCore.Helpers;
using AutoMapper;

namespace ApplicationCore.Services
{
	public interface IQuestionsService
	{
		Task<IEnumerable<Question>> FetchAsync(Subject subject, ICollection<int> termIds = null, ICollection<int> recruitIds = null);
		Task<Question> GetByIdAsync(int id);
		Task<Question> CreateAsync(Question question);
		Task UpdateAsync(Question question);
		Task RemoveAsync(Question question);

		Task UpdateAsync(Question existingEntity, Question model);

		Question GetById(int id);
	}

	public class QuestionsService : IQuestionsService
	{
		private readonly IDefaultRepository<Question> _questionRepository;
		private readonly IDefaultRepository<Option> _optionRepository;
		private readonly DefaultContext _context;

		public QuestionsService(IDefaultRepository<Question> questionRepository, IDefaultRepository<Option> optionRepository, DefaultContext context)
		{
			_questionRepository = questionRepository;
			_optionRepository = optionRepository;
			_context = context;
		}

		public async Task<IEnumerable<Question>> FetchAsync(Subject subject, ICollection<int> termIds = null, ICollection<int> recruitIds = null)
		{
			var spec = new QuestionFilterSpecification(subject);
			var list = await _questionRepository.ListAsync(spec);

			if (termIds.HasItems())
			{
				var questionIds = FetchQuestionIdsByTerms(termIds);
				list = list.Where(item => questionIds.Contains(item.Id)).ToList();
			}

			if (recruitIds.HasItems())
			{
				var questionIds = FetchQuestionIdsByRecruits(recruitIds);
				list = list.Where(item => questionIds.Contains(item.Id)).ToList();
			}

			

			return list;
		}

		public async Task<Question> GetByIdAsync(int id) => await _questionRepository.GetByIdAsync(id);

		public async Task<Question> CreateAsync(Question question)
		{
			question = await _questionRepository.AddAsync(question);

			if (!String.IsNullOrEmpty(question.TermIds))
			{
				await AddTermQuestionsAsync(question);
			}

			return question;
		}
		

		public async Task UpdateAsync(Question existingEntity, Question question)
		{
			existingEntity.RecruitQuestions = question.RecruitQuestions;

			await _questionRepository.UpdateAsync(existingEntity, question);
			
			_optionRepository.SyncList(existingEntity.Options.ToList(), question.Options.ToList());

			await SyncTermQuestions(question);

		}

		public async Task UpdateAsync(Question question) => await _questionRepository.UpdateAsync(question);

		public Question GetById(int id)
		{
			var spec = new QuestionFilterSpecification(id);
			return _questionRepository.GetSingleBySpec(spec);
		}

		public async Task RemoveAsync(Question question)
		{
			question.Removed = true;
			await _questionRepository.UpdateAsync(question);
		}

		IEnumerable<int> FetchQuestionIdsByRecruits(ICollection<int> recruitIds)
		{
			var recruitQuestions = _context.RecruitQuestions.Where(x => recruitIds.Contains(x.RecruitId));
			if (recruitQuestions.IsNullOrEmpty()) return new List<int>();

			return recruitQuestions.Select(x => x.QuestionId).ToList();
		}

		IEnumerable<int> FetchQuestionIdsByTerms(ICollection<int> termIds)
		{
			var termQuestions = _context.TermQuestions.Where(x => termIds.Contains(x.TermId));
			if (termQuestions.IsNullOrEmpty()) return new List<int>();

			return termQuestions.Select(x => x.QuestionId).ToList();
		}



		async Task SyncTermQuestions(Question entity)
		{
			await RemoveTermQuestionsAsync(entity);
			if(!String.IsNullOrEmpty(entity.TermIds)) await AddTermQuestionsAsync(entity);
		}

		async Task RemoveTermQuestionsAsync(Question entity)
		{
			_context.TermQuestions.RemoveRange(_context.TermQuestions.Where(x => x.QuestionId == entity.Id).ToList());
			await _context.SaveChangesAsync();
		}

		async Task AddTermQuestionsAsync(Question entity)
		{
			var termIds = entity.TermIds.SplitToIds();
			var termQuestions = termIds.Select(termId => new TermQuestion { QuestionId = entity.Id, TermId = termId });

			_context.TermQuestions.AddRange(termQuestions);
			await _context.SaveChangesAsync();
		}
	}
}
