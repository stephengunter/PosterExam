using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Services
{
    public interface IDataService
    {
        string Test();
    }

    public class DataService : IDataService
    {
        private readonly AppSettings _settings;

        public DataService(IOptions<AppSettings> settings)
        {
            _settings = settings.Value;
        }

        public string Test()
        {
            return _settings.DataPath;
        }
    }
}
