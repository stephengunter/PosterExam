using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;


namespace ApplicationCore.Specifications
{
	public class AttachmentFilterSpecifications : BaseSpecification<UploadFile>
	{
		public AttachmentFilterSpecifications(PostType postType)
			: base(a => a.PostType == postType)
		{

		}


		public AttachmentFilterSpecifications(PostType postType, int postId)
			: base(a => a.PostType == postType && a.PostId == postId)
		{

		}

		public AttachmentFilterSpecifications(IList<int> ids) : base(a => ids.Contains(a.Id))
		{

		}
	}

}
