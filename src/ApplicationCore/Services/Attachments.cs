using System.Collections.Generic;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Helpers;

namespace ApplicationCore.Services
{
	public interface IAttachmentsService
	{
		Task<IEnumerable<UploadFile>> FetchAsync(PostType postType, int postId = 0);

		Task<IEnumerable<UploadFile>> FetchAsync(PostType postType, IList<int> postIds);

		Task<UploadFile> FindByNameAsync(string name, PostType postType, int postId);

		Task<IEnumerable<UploadFile>> FetchByIdsAsync(IList<int> ids);

		Task<UploadFile> CreateAsync(UploadFile attachment);

		Task<UploadFile> GetByIdAsync(int id);

		Task UpdateAsync(UploadFile attachment);

		void UpdateRange(IEnumerable<UploadFile> attachments);

		void DeleteRange(IEnumerable<UploadFile> attachments);

		Task DeleteAsync(UploadFile attachment);

		Task LoadAttachmentsAsync(Option option);

		Task SyncAttachmentsAsync(Option option, ICollection<UploadFile> latestList);

	}

	public class AttachmentsService : IAttachmentsService
	{
		private readonly IDefaultRepository<UploadFile> _uploadFileRepository;

		public AttachmentsService(IDefaultRepository<UploadFile> uploadFileRepository)
		{
			this._uploadFileRepository = uploadFileRepository;
		}

		public async Task<IEnumerable<UploadFile>> FetchAsync(PostType postType, int postId = 0)
		{
			if (postId > 0)
			{
				var filter = new AttachmentFilterSpecifications(postType, postId);
				return await _uploadFileRepository.ListAsync(filter);
			}

			return await _uploadFileRepository.ListAsync(new AttachmentFilterSpecifications(postType));

		}

		public async Task<IEnumerable<UploadFile>> FetchAsync(PostType postType, IList<int> postIds)
		{ 
			var list = await _uploadFileRepository.ListAsync(new AttachmentFilterSpecifications(postType));

			return list.Where(x => postIds.Contains(x.PostId));
		}

		public async Task<UploadFile> FindByNameAsync(string name, PostType postType, int postId)
		{
			var attachments = await FetchAsync(postType, postId);
			if (attachments.IsNullOrEmpty()) return null;

			return attachments.Where(a => a.Name == name).FirstOrDefault();
		}

		public async Task<IEnumerable<UploadFile>> FetchByIdsAsync(IList<int> ids)
		{
			var filter = new AttachmentFilterSpecifications(ids);

			return await _uploadFileRepository.ListAsync(filter);
		}

		public async Task<UploadFile> GetByIdAsync(int id) => await _uploadFileRepository.GetByIdAsync(id);

		public async Task<UploadFile> CreateAsync(UploadFile attachment) => await _uploadFileRepository.AddAsync(attachment);

		public async Task UpdateAsync(UploadFile attachment) => await _uploadFileRepository.UpdateAsync(attachment);

		public async Task DeleteAsync(UploadFile attachment) => await _uploadFileRepository.DeleteAsync(attachment);

		public void UpdateRange(IEnumerable<UploadFile> attachments) => _uploadFileRepository.UpdateRange(attachments);

		public void DeleteRange(IEnumerable<UploadFile> attachments) => _uploadFileRepository.DeleteRange(attachments);

		public async Task LoadAttachmentsAsync(Option option)
		{
			var attachments = await FetchAsync(PostType.Option, option.Id);

			option.Attachments = attachments.HasItems() ? attachments.ToList() : new List<UploadFile>();

		}

		public async Task SyncAttachmentsAsync(Option option, ICollection<UploadFile> latestList)
		{
			var existingList = await FetchAsync(PostType.Option, option.Id);

			SyncAttachments(existingList.ToList(), latestList);
		}

		void SyncAttachments(ICollection<UploadFile> existingList, ICollection<UploadFile> latestList)
		{
			if (latestList.IsNullOrEmpty()) latestList = new List<UploadFile>();

			foreach (var existingItem in existingList)
			{
				if (!latestList.Any(item => item.Id == existingItem.Id))
				{
					existingItem.Removed = true;
				}
			}

			foreach (var latestItem in latestList)
			{
				var existingItem = existingList.Where(item => item.Id == latestItem.Id).FirstOrDefault();

				if (existingItem != null) _uploadFileRepository.DbContext.Entry(existingItem).CurrentValues.SetValues(latestItem);
				else _uploadFileRepository.DbSet.Add(latestItem);

			}

			_uploadFileRepository.DbContext.SaveChanges();

		}

	}
}
