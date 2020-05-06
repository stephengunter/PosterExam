using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Views;
using ApplicationCore.Settings;
using Infrastructure.Views;
using ApplicationCore.Paging;

namespace Web.Models
{
    public class ExceptionsIndexModel
    {
        public string StartDateText { get; set; }

        public string EndDateText { get; set; }

        public PagedList<ExceptionViewModel> PagedList { get; set; }

        public ICollection<BaseOption<string>> TypeOptions { get; set; } = new List<BaseOption<string>>();


        public void LoadTypeOptions(IEnumerable<string> types)
            => TypeOptions = types.Select(type => new BaseOption<string>(type, type)).ToList();


    }
}
