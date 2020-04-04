﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using ApplicationCore.Helpers;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace ApplicationCore.Models
{
	public class Term : BaseCategory
	{
		public bool ChapterTitle { get; set; }

		public int SubjectId { get; set; }

		public string Text { get; set; }

		public Subject Subject { get; set; }

		public ICollection<Note> Notes { get; private set; }

		public bool Hide { get; set; }

		public string QIds { get; set; } //精選試題

		public string Highlight { get; set; } //json string

		public string Reference { get; set; } //json string

		

		[NotMapped]
		public ICollection<Term> SubItems { get; private set; }
		

		[NotMapped]
		public ICollection<int> ParentIds { get; private set; } = new List<int>();

		#region Helpers
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

		public ICollection<int> GetQIds()
		{
			var qids = new List<int>();
			if(!String.IsNullOrEmpty(QIds)) qids.AddRange(QIds.SplitToIds());

			foreach (var item in SubItems)
			{
				qids.AddRange(item.QIds.SplitToIds());
				qids.AddRange(item.GetQIds());
			}
			return qids;
		}


		public void LoadSubItems(IEnumerable<Term> subItems)
		{
			SubItems = subItems.Where(item => item.ParentId == this.Id).OrderBy(item => item.Order).ToList();

			foreach (var item in SubItems)
			{
				item.LoadSubItems(subItems);
			}
		}

		public void LoadNotes(IEnumerable<Note> notes)
		{
			Notes = notes.Where(item => item.TermId == this.Id).OrderBy(item => item.Order).ToList();

			foreach (var item in SubItems)
			{
				item.LoadNotes(notes);
			}
		}

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

		#endregion

	}


}
