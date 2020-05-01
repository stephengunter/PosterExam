using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayMvc.Views
{
    public class ExceptionViewModel
    {
        public ExceptionViewModel() { }

        public ExceptionViewModel(Exception ex)
        {
            TypeName = ex.GetType().Name;
            Content = $"{ex}";
        }

        public string Id = Guid.NewGuid().ToString();

        public string TypeName { get; set; }

        public string Content { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;

    }
}