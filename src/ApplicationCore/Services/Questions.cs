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
		Task<IEnumerable<Question>> FetchAsync(Subject subject, ICollection<int> termIds = null);
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

		public QuestionsService(IDefaultRepository<Question> questionRepository, IDefaultRepository<Option> optionRepository)
		{
			_questionRepository = questionRepository;
			_optionRepository = optionRepository;
		}

		public async Task<IEnumerable<Question>> FetchAsync(Subject subject, ICollection<int> termIds = null)
		{
			QuestionFilterSpecification spec;
			if (termIds.IsNullOrEmpty()) spec = new QuestionFilterSpecification(subject);
			else spec = new QuestionFilterSpecification(subject, termIds);

			return await _questionRepository.ListAsync(spec);
		}

		public async Task<Question> GetByIdAsync(int id) => await _questionRepository.GetByIdAsync(id);

		public async Task<Question> CreateAsync(Question question) => await _questionRepository.AddAsync(question);

		public async Task UpdateAsync(Question existingEntity, Question model)
		{
			existingEntity.RecruitQuestions = model.RecruitQuestions;

			await _questionRepository.UpdateAsync(existingEntity, model);

			_optionRepository.SyncList(existingEntity.Options.ToList(), model.Options.ToList());

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


	}
}
