using Newtonsoft.Json;
using PayMvc.Helpers;
using PayMvc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace PayMvc.Logging
{
    public interface IAppLogger
    {
        void LogInfo(string msg);
        void LogException(Exception ex);
        List<ExceptionViewModel> FetchAllExceptions();

        Task LogRequestAsync(HttpRequestMessage httpRequest);
        List<RequestViewModel> FetchAllRequests();

        void LogReport(string url, string content, int status = 0);
        List<ReportViewModel> FetchAllReports();
    }

    public class AppLogger : IAppLogger
    {
        private readonly NLog.ILogger _logger;
        private string _exceptionsFilePath;
        private string _requestsFilePath;
        private string _reportsFilePath;

        public AppLogger(string folderPath, NLog.ILogger logger)
        {
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            //exceptions
            InitExceptionsFile(folderPath);

            //requests
            InitRequestsFile(folderPath);

            //reports
            InitReportsFile(folderPath);

            _logger = logger;
        }

        void InitExceptionsFile(string folderPath)
        {
            //exceptions
            var exceptionsfilePath = Path.Combine(folderPath, "exceptions.json");
            if (!File.Exists(exceptionsfilePath)) File.Create(exceptionsfilePath);
            _exceptionsFilePath = exceptionsfilePath;
        }

        void InitRequestsFile(string folderPath)
        {
            var requestsfilePath = Path.Combine(folderPath, "requests.json");
            if (!File.Exists(requestsfilePath)) File.Create(requestsfilePath);
            _requestsFilePath = requestsfilePath;
        }

        void InitReportsFile(string folderPath)
        {
            var reportsfilePath = Path.Combine(folderPath, "reports.json");
            if (!File.Exists(reportsfilePath)) File.Create(reportsfilePath);
            _reportsFilePath = reportsfilePath;
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

            File.WriteAllText(_exceptionsFilePath, JsonConvert.SerializeObject(list));
        }

        public List<ExceptionViewModel> FetchAllExceptions()
        {
            var doc = File.ReadAllText(_exceptionsFilePath);

            var list = JsonConvert.DeserializeObject<List<ExceptionViewModel>>(doc);


            return list.IsNullOrEmpty() ? new List<ExceptionViewModel>() : list;
        }

        public async Task LogRequestAsync(HttpRequestMessage httpRequest)
        {
            var content = await httpRequest.Content.ReadAsStringAsync();
            var model = new RequestViewModel
            { 
                Origin = httpRequest.GetOrigin(),
                Content = content
            
            };

            var list = FetchAllRequests();
            list.Add(model);

            File.WriteAllText(_requestsFilePath, JsonConvert.SerializeObject(list));
        }


        public List<RequestViewModel> FetchAllRequests()
        {
            var doc = File.ReadAllText(_requestsFilePath);

            var list = JsonConvert.DeserializeObject<List<RequestViewModel>>(doc);


            return list.IsNullOrEmpty() ? new List<RequestViewModel>() : list;
        }

        public void LogReport(string url, string content, int status = 0)
        {
            var model = new ReportViewModel
            {
                Url = url,
                Status = status,
                Content = content
            };

            var list = FetchAllReports();
            list.Add(model);

            File.WriteAllText(_reportsFilePath, JsonConvert.SerializeObject(list));
        }


        public List<ReportViewModel> FetchAllReports()
        {
            var doc = File.ReadAllText(_reportsFilePath);

            var list = JsonConvert.DeserializeObject<List<ReportViewModel>>(doc);

            return list.IsNullOrEmpty() ? new List<ReportViewModel>() : list;
        }
    }
}