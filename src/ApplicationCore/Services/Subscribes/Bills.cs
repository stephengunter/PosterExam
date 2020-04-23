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
	public interface IBillsService
	{
		Task<IEnumerable<Bill>> FetchAsync(bool active);
		Task<IEnumerable<Bill>> FetchByUserAsync(string userId);
		Task<IEnumerable<Bill>> FetchByUserAsync(User user, Plan plan);

		Task<IEnumerable<Bill>> FetchAllAsync();
		Bill GetById(int id);
		Task<Bill> CreateAsync(Bill bill);
	}

	public class BillsService : IBillsService
	{
		private readonly IDefaultRepository<Bill> _billRepository;

		public BillsService(IDefaultRepository<Bill> billRepository)
		{
			this._billRepository = billRepository;
		}

		public async Task<Bill> CreateAsync(Bill bill) => await _billRepository.AddAsync(bill);

		public async Task<IEnumerable<Bill>> FetchAsync(bool active)
		{
			var bills = await FetchAllAsync();
			if (bills.IsNullOrEmpty()) return null;

			return bills.Where(x => x.Active == active);
		}

		public async Task<IEnumerable<Bill>> FetchByUserAsync(string userId)
		{
			var spec = new BillFilterSpecification(new User { Id = userId });
			return await _billRepository.ListAsync(spec);
		}

		public async Task<IEnumerable<Bill>> FetchByUserAsync(User user, Plan plan)
		{
			var spec = new BillFilterSpecification(user, plan);
			return await _billRepository.ListAsync(spec);
		}

		public async Task<IEnumerable<Bill>> FetchAllAsync() => await _billRepository.ListAsync(new BillFilterSpecification());

		public Bill GetById(int id)
		{
			var spec = new BillFilterSpecification(id);
			return _billRepository.GetSingleBySpec(spec);
		}
	}
}
