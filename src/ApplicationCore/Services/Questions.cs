using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ApplicationCore.Helpers;

namespace ApplicationCore.Services
{
	public interface IQuestionsService
	{
		Task<IEnumerable<Question>> FetchAsync(int subjectId);
		Task<Question> GetByIdAsync(int id);
		Task<Question> CreateAsync(Question question);
		Task UpdateAsync(Question question);
		Task RemoveAsync(Question question);
	}

	public class QuestionsService : IQuestionsService
	{
		private readonly IDefaultRepository<Question> _questionRepository;

		public QuestionsService(IDefaultRepository<Question> questionRepository)
		{
			this._questionRepository = questionRepository;
		}

		public async Task<IEnumerable<Question>> FetchAsync(int subjectId)
		{
			var spec = new QuestionFilterSpecification(subjectId);
			var list = await _questionRepository.ListAsync(spec);

			
			return list;
		}

		public async Task<Question> GetByIdAsync(int id) => await _questionRepository.GetByIdAsync(id);

		public async Task<Question> CreateAsync(Question Question) => await _questionRepository.AddAsync(Question);

		public async Task UpdateAsync(Question Question) => await _questionRepository.UpdateAsync(Question);

		

		public async Task RemoveAsync(Question Question)
		{
			Question.Removed = true;
			await _questionRepository.UpdateAsync(Question);
		}
		
		
	}
}
