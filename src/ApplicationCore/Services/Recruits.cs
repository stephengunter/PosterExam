﻿using ApplicationCore.DataAccess;
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
	public interface IRecruitsService
	{
		Task<IEnumerable<Recruit>> GetAllAsync();
		Task<IEnumerable<Recruit>> FetchAsync(bool active);
		Task<Recruit> GetByIdAsync(int id);
		Task<Recruit> CreateAsync(Recruit recruit);
		Task UpdateAsync(Recruit recruit);
		Task UpdateAsync(Recruit existingEntity, Recruit model);
		Task RemoveAsync(Recruit recruit);

		Recruit GetById(int id);
	}

	public class RecruitsService : IRecruitsService
	{
		private readonly IDefaultRepository<Recruit> _recruitRepository;

		public RecruitsService(IDefaultRepository<Recruit> recruitRepository)
		{
			this._recruitRepository = recruitRepository;
		}

		public async Task<IEnumerable<Recruit>> GetAllAsync() => await _recruitRepository.ListAsync(new RecruitFilterSpecification());

		public async Task<IEnumerable<Recruit>> FetchAsync(bool active) => await _recruitRepository.ListAsync(new RecruitFilterSpecification(active));
		
		public async Task<Recruit> GetByIdAsync(int id) => await _recruitRepository.GetByIdAsync(id);

		public async Task<Recruit> CreateAsync(Recruit recruit) => await _recruitRepository.AddAsync(recruit);

		public async Task UpdateAsync(Recruit recruit) => await _recruitRepository.UpdateAsync(recruit);

		public async Task UpdateAsync(Recruit existingEntity, Recruit model) => await _recruitRepository.UpdateAsync(existingEntity, model);

		public async Task RemoveAsync(Recruit recruit)
		{
			recruit.Removed = true;
			await _recruitRepository.UpdateAsync(recruit);
		}

		public Recruit GetById(int id) => _recruitRepository.GetById(id);
	}
}
