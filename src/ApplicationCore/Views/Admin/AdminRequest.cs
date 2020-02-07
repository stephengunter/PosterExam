using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ApplicationCore.Views
{
	public class AdminRequest
	{
		public string Key { get; set; }
		public string Cmd { get; set; }
		public List<IFormFile> Files { get; set; }


		public IFormFile GetFile(string name) => Files.FirstOrDefault(item => Path.GetFileNameWithoutExtension(item.FileName) == name);
	}

}
