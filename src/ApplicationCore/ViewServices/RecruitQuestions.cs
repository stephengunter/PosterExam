using ApplicationCore.Views;
using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Helpers;
using System.Linq;

namespace ApplicationCore.ViewServices
{
    public static class RecruitQuestionsViewService
    {
        //public static List<T> ToList<T>() => Enum.GetValues(typeof(T)).Cast<T>().ToList();

        public static void LoadModeOptions(this RQIndexViewModel model)
        {
            model.ModeOptions = GetModeOptions();
        }


        public static ICollection<BaseOption<int>> GetModeOptions()
        {
            var options = new List<BaseOption<int>>();
            foreach (RQMode mode in (RQMode[])Enum.GetValues(typeof(RQMode)))
            {
                string text = mode.GetDisplayName();
                if (!String.IsNullOrEmpty(text))
                {
                    options.Add(new BaseOption<int>((int)mode, text));
                }
                
            }
            return options;
        }

        public static string GetDisplayName(this RQMode model)
        {
            if (model == RQMode.Read) return "閱讀";
            if (model == RQMode.Exam) return "測驗";
            return "";
        }
    }
}
