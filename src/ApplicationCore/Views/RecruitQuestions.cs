using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Views;

namespace ApplicationCore.Views
{
    public enum RQMode
    {
        Read = 0,
        Exam = 1,
        Unknown = -1
    }

    public class RQIndexViewModel
    {
        public ICollection<BaseOption<int>> ModeOptions { get; set; }
    }
}
