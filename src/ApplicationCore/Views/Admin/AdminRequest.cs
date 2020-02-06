using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Views
{
	public class AdminRequest
	{
		public string Key { get; set; }
		public string Cmd { get; set; }
		public List<IFormFile> Files { get; set; }
	}

}
