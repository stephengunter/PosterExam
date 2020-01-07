﻿using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models
{
	public enum PostType
	{
		Option
	}

	public class UploadFile : BaseUploadFile
	{
		public PostType PostType { get; set; }
		public int PostId { get; set; }

	}
}
