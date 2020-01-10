using ApplicationCore.Views;
using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Helpers;
using System.Linq;
using ApplicationCore.Models;
using ApplicationCore.ViewServices;
using Web.Models;

namespace Web.Helpers
{
    public static class RecruitQuestionsViewService
    {
        public static void LoadModeOptions(this RQIndexViewModel model)
        {
            model.ModeOptions = GetModeOptions();
        }

        public static void LoadYearOptions(this RQIndexViewModel model, IEnumerable<Recruit> recruits)
        {
            var years = recruits.Where(x => x.RecruitEntityType == RecruitEntityType.Year).GetOrdered();
            model.YearOptions = years.Select(item => item.ToOption()).ToList();
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

        public static RQMode ToRQModeType(this int val)
        {
            try
            {

                if (Enum.IsDefined(typeof(RQMode), val))
                {
                    RQMode type = (RQMode)val;
                    return type;
                }
                return RQMode.Unknown;


            }
            catch (Exception ex)
            {
                return RQMode.Unknown;
            }
        }
    }
}
