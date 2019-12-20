using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace ApplicationCore.Models
{
	public class Term : BaseCategory
	{
		public int SubjectId { get; set; }

		public string Text { get; set; }

		public Subject Subject { get; set; }

		[NotMapped]
		public ICollection<Term> SubItems { get; private set; }

		//[NotMapped]
		//public Term RootItem { get; private set; }

		[NotMapped]
		public ICollection<int> ParentIds { get; private set; } = new List<int>();


		public ICollection<int> GetSubIds()
		{
			var subIds = new List<int>();
			foreach (var item in SubItems)
			{
				subIds.Add(item.Id);

				subIds.AddRange(item.GetSubIds());
			}
			return subIds;
		}


		public void LoadSubItems(IEnumerable<Term> subItems)
		{
			SubItems = subItems.Where(item => item.ParentId == this.Id).OrderBy(item => item.Order).ToList();

			foreach (var item in SubItems)
			{
				item.LoadSubItems(subItems);
			}
		}

		//public void FindRoot(IEnumerable<Term> allTerms)
		//{
		//	Term root = null;

		//	if (ParentId > 0)
		//	{
		//		int parentId = ParentId;
				
		//		do
		//		{
		//			var parent = allTerms.Where(item => item.Id == parentId).FirstOrDefault();
		//			if (parent == null) throw new Exception($"Term not found. id = {parentId}");

		//			if (parent.IsRootItem) root = parent;
		//			parentId = parent.Id;

		//		} while (root == null);
		//	}

		//	RootItem = root;
		//}

		public void LoadParentIds(IEnumerable<Term> allTerms)
		{
			var parentIds = new List<int>();
			Term root = null;
			if (ParentId > 0)
			{
				int parentId = ParentId;

				do
				{
					var parent = allTerms.Where(item => item.Id == parentId).FirstOrDefault();
					if (parent == null) throw new Exception($"Term not found. id = {parentId}");

					if (parent.IsRootItem) root = parent;
					parentId = parent.Id;
					parentIds.Insert(0, parentId);

				} while (root == null);
			}


			ParentIds = parentIds;
		}
	}
}
