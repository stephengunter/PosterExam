﻿using Infrastructure.Entities;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models
{
	public class OAuth : BaseEntity, IAggregateRoot
	{
		public string UserId { get; set; }

		public string OAuthId { get; set; }

		public OAuthProvider Provider { get; set; }

		public string PictureUrl { get; set; }
		

		public User User { get; set; }
	}

	public enum OAuthProvider
	{
		Google,
		Facebook,
		Wechat,
		Line
	}
}
