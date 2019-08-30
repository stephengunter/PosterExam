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
	public interface ITermsService
	{
		Task<IEnumerable<Term>> FetchAsync(int subjectId, int parentId);
		Task<Term> GetByIdAsync(int id);
		Task<Term> CreateAsync(Term term);
		Task UpdateAsync(Term term);
		Task RemoveAsync(Term term);


		Term GetById(int id);

		Task<int> GetMaxOrderAsync(int subjectId, int parentId);
	}

	public class TermsService : BaseCategoriesService<Term>, ITermsService
	{
		private readonly IDefaultRepository<Term> _termRepository;

		public TermsService(IDefaultRepository<Term> termRepository)
		{
			this._termRepository = termRepository;
		}

		public async Task<IEnumerable<Term>> FetchAsync(int subjectId, int parentId)
		{
			var spec = new TermFilterSpecification(subjectId, parentId);
			var list = await _termRepository.ListAsync(spec);

			LoadSubItems(list);
			return list;
		}

		public async Task<Term> GetByIdAsync(int id) => await _termRepository.GetByIdAsync(id);

		public async Task<Term> CreateAsync(Term term) => await _termRepository.AddAsync(term);

		public async Task UpdateAsync(Term term) => await _termRepository.UpdateAsync(term);

		

		public async Task RemoveAsync(Term term)
		{
			term.Removed = true;
			await _termRepository.UpdateAsync(term);
		}

		public Term GetById(int id)
		{
			var term = _termRepository.GetById(id);
			if (term == null) return null;

			LoadSubItems(term);

			return term;

		}

		public async Task<int> GetMaxOrderAsync(int subjectId, int parentId)
		{
			var spec = new TermFilterSpecification(subjectId, parentId);
			var list = await _termRepository.ListAsync(spec);

			if (list.IsNullOrEmpty()) return 0;
			return list.Max(item => item.Order);
		}

		void LoadSubItems(IEnumerable<Term> list)
		{
			if (list.IsNullOrEmpty()) return;

			var subItems = AllSubItems(_termRepository.DbSet);


			foreach (var entity in list)
			{
				entity.LoadSubItems(subItems.ToList());
			}
		}

		void LoadSubItems(Term entity)
		{
			var subItems = AllSubItems(_termRepository.DbSet);

			entity.LoadSubItems(subItems.ToList());
		}
	}
}
