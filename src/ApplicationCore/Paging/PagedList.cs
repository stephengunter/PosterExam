using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationCore.Helpers;

namespace ApplicationCore.Paging
{
	public interface IPagedList<T,V>
	{
		List<T> List { get; set; }
		List<V> ViewList { get; set; }

		int TotalItems { get; set; }
		int PageNumber { get; }
		int PageSize { get; }
		


		int TotalPages { get; }
		
		bool HasPreviousPage { get; }
		bool HasNextPage { get; }
		int NextPageNumber { get; }
		int PreviousPageNumber { get; }

		IPagingHeader GetHeader();

	}
	public class PagedList<T,V> : IPagedList<T,V>
	{
		
		public PagedList(IEnumerable<T> list, int pageNumber=1, int pageSize=999)
		{
			this.TotalItems = list.Count();
			this.PageNumber = pageNumber;
			this.PageSize = pageSize;

			this.List = list.GetPaged(pageNumber, pageSize).ToList();
			this.ViewList = new List<V>();

			this.TotalPages= (int)Math.Ceiling(this.TotalItems / (double)this.PageSize);

			this.HasPreviousPage= this.PageNumber > 1;
			this.HasNextPage = this.PageNumber < this.TotalPages;
			this.NextPageNumber= this.HasNextPage ? this.PageNumber + 1 : this.TotalPages;
			this.PreviousPageNumber = this.HasPreviousPage ? this.PageNumber - 1 : 1;


		}

		

		public List<T> List { get; set; }
		public List<V> ViewList { get; set; }

		public int TotalItems { get; set; }
		public int PageNumber { get; }
		public int PageSize { get; }
		public int TotalPages { get; }
		public bool HasPreviousPage { get; }
		public bool HasNextPage { get; }

		public int NextPageNumber { get; }

		public int PreviousPageNumber { get; }

		public IPagingHeader GetHeader()
		{
			return new PagingHeader(this.TotalItems, this.PageNumber, this.PageSize, this.TotalPages);
		}
	}
}
