using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Views
{
	public class RefreshTokenRequest
	{
		public string accessToken { get; set; }
		public string refreshToken { get; set; }

	}

	public class OAuthLoginRequest
	{
		public string token { get; set; }
		
	}
}
