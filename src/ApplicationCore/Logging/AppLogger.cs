using ApplicationCore.Helpers;
using ApplicationCore.Settings;
using ApplicationCore.Views;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ApplicationCore.Logging
{
	public interface IAppLogger
	{
		void LogInfo(string message);
		void LogWarn(string message);
		void LogDebug(string message);
		void LogError(string message);

		void LogException(Exception ex);
		List<ExceptionViewModel> FetchAllExceptions();

	}

	public class AppLogger : IAppLogger
	{
		private static readonly NLog.ILogger logger = LogManager.GetCurrentClassLogger();

		private readonly AdminSettings _adminSettings;
		private readonly string _filePath;

		public AppLogger(IOptions<AdminSettings> adminSettings)
		{
			_adminSettings = adminSettings.Value;
			var folderPath = _adminSettings.DataPath;
			if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

			var filePath = Path.Combine(folderPath, "exceptions.json");
			if (!System.IO.File.Exists(filePath)) System.IO.File.Create(filePath);

			_filePath = filePath;
		}

		

		public void LogDebug(string message)
		{
			logger.Debug(message);
		}

		public void LogError(string message)
		{
			logger.Error(message);
		}

		public void LogException(Exception ex)
		{
			SaveException(new ExceptionViewModel(ex));

			logger.Error(ex);
		}

		public void LogInfo(string message)
		{
			logger.Info(message);
		}

		public void LogWarn(string message)
		{
			logger.Warn(message);
		}


		public void SaveException(ExceptionViewModel model)
		{
			var list = FetchAllExceptions();
			list.Add(model);

			System.IO.File.WriteAllText(_filePath, JsonConvert.SerializeObject(list));
		}

		public List<ExceptionViewModel> FetchAllExceptions()
		{
			var doc = System.IO.File.ReadAllText(_filePath);

			var list = JsonConvert.DeserializeObject<List<ExceptionViewModel>>(doc);


			return list.IsNullOrEmpty() ? new List<ExceptionViewModel>() : list;
		}

	}
}
