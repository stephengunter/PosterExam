using Newtonsoft.Json;
using PayMvc.Helpers;
using PayMvc.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PayMvc.Logging
{
    public interface IAppLogger
    {
        void LogInfo(string msg);
        void LogException(Exception ex);
        List<ExceptionViewModel> FetchAllExceptions();
    }

    public class AppLogger : IAppLogger
    {
        private readonly NLog.ILogger _logger;
        private readonly string _filePath;

        public AppLogger(string folderPath, NLog.ILogger logger)
        {
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, "exceptions.json");
            if (!File.Exists(filePath)) File.Create(filePath);

            _filePath = filePath;

            _logger = logger;
        }

        public void LogInfo(string msg)
        {
            _logger.Info(msg);
        }

        public void LogException(Exception ex)
        {
            _logger.Error(ex);

            var model = new ExceptionViewModel(ex);

            var list = FetchAllExceptions();
            list.Add(model);

            File.WriteAllText(_filePath, JsonConvert.SerializeObject(list));
        }

        public List<ExceptionViewModel> FetchAllExceptions()
        {
            var doc = File.ReadAllText(_filePath);

            var list = JsonConvert.DeserializeObject<List<ExceptionViewModel>>(doc);


            return list.IsNullOrEmpty() ? new List<ExceptionViewModel>() : list;
        }

        
    }
}