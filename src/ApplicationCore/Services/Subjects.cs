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
	public interface ISubjectsService
	{
		Task<IEnumerable<Subject>> FetchAsync();
		Task<Subject> GetByIdAsync(int id);
		Task<Subject> CreateAsync(Subject subject);
		Task UpdateAsync(Subject subject);
		Task RemoveAsync(Subject subject);


		Subject GetById(int id);
	}

	public class SubjectsService : BaseCategoriesService<Subject>, ISubjectsService
	{
		private readonly IDefaultRepository<Subject> _subjectRepository;

		public SubjectsService(IDefaultRepository<Subject> subjectRepository)
		{
			this._subjectRepository = subjectRepository;
		}

		public async Task<IEnumerable<Subject>> FetchAsync()
		{
			int parentId = 0;
			var spec = new SubjectFilterSpecification(parentId);
			var list = await _subjectRepository.ListAsync(spec);

			LoadSubItems(list);
			return list;
		}

		public async Task<Subject> GetByIdAsync(int id) => await _subjectRepository.GetByIdAsync(id);

		public async Task<Subject> CreateAsync(Subject subject) => await _subjectRepository.AddAsync(subject);

		public async Task UpdateAsync(Subject subject) => await _subjectRepository.UpdateAsync(subject);

		public async Task RemoveAsync(Subject subject)
		{
			subject.Removed = true;
			await _subjectRepository.UpdateAsync(subject);
		}

		public Subject GetById(int id)
		{
			var subject = _subjectRepository.GetById(id);
			if (subject == null) return null;

			LoadSubItems(subject);

			return subject;

		}
		
		


		void LoadSubItems(IEnumerable<Subject> list)
		{
			if (list.IsNullOrEmpty()) return;

			var subItems = AllSubItems(_subjectRepository.DbSet);


			foreach (var entity in list)
			{
				entity.LoadSubItems(subItems.ToList());
			}
		}

		void LoadSubItems(Subject entity)
		{
			var subItems = AllSubItems(_subjectRepository.DbSet);

			entity.LoadSubItems(subItems.ToList());
		}
	}
}
