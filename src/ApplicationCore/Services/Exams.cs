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
	public interface IExamsService
	{
		
		Task<Exam> CreateAsync(Exam exam, string userId);
		Task<Exam> GetByIdAsync(int id);
		Task UpdateAsync(Exam exam);
		Task RemoveAsync(Exam exam);
		

	}

	public class ExamsService : IExamsService
	{
		private readonly IDefaultRepository<Exam> _examRepository;
		private readonly IDefaultRepository<Option> _optionRepository;
		private readonly DefaultContext _context;

		public ExamsService(IDefaultRepository<Exam> examRepository, IDefaultRepository<Option> optionRepository,
			DefaultContext context)
		{
			_examRepository = examRepository;
			_optionRepository = optionRepository;
			_context = context;
		}

		

		public async Task<Exam> GetByIdAsync(int id) => await _examRepository.GetByIdAsync(id);

		public async Task<Exam> CreateAsync(Exam exam, string userId)
		{
			exam.UserId = userId;
			exam.SetCreated(userId);
			return await _examRepository.AddAsync(exam);
		}


		public async Task UpdateAsync(Exam exam) => await _examRepository.UpdateAsync(exam);

		

		public async Task RemoveAsync(Exam exam)
		{
			exam.Removed = true;
			await _examRepository.UpdateAsync(exam);
		}

		

	}
}
