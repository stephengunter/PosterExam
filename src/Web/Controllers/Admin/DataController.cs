using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.ViewServices;
using Web.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using ApplicationCore.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Data.SqlClient;
using Infrastructure.Entities;
using System.Data;
using Microsoft.AspNetCore.Http;

namespace Web.Controllers.Admin
{
	public class DataController : BaseAdminController
	{
		private readonly IDataService _dataService;

		public DataController(IDataService dataService)
		{
			_dataService = dataService;
		}

		
		//void SaveJson(string folderPath, string name, string content)
		//{
		//	var filePath = Path.Combine(folderPath, $"{name}.json");
		//	System.IO.File.WriteAllText(filePath, content);
		//}

		//string DataFolder()
		//{
		//	var path = Path.Combine(_adminSettings.BackupPath, DateTime.Today.ToDateNumber().ToString());
		//	if (!Directory.Exists(path)) Directory.CreateDirectory(path);

		//	return path;
		//}

	}
}
