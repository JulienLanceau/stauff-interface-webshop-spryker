using SAPbobsCOM;
using stauff_interface_webshop_spryker_ui.Configuration;
using stauff_interface_webshop_spryker_ui.DataContrats.In;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Runtime.InteropServices;

namespace stauff_interface_webshop_spryker_ui.Extensions {
    public static class Extensions {
        //private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static string CutIfMoreThan(this string str, int i) {
            if(str != null && str.Length > i) {
                return str[..i];
            }
            return str;
        }
        public static void ReleaseComObject(this object @object) {
            if(@object == null)
                return;
            try {
                GC.WaitForPendingFinalizers();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(@object);
                @object = null;
            } catch {
            }
        }

        static readonly IEnumerable<RegionInfo> regions = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(x => new RegionInfo(x.LCID));
        public static string CountryNameFromTwoLetterISO(this string code) {
            var englishRegion = regions.FirstOrDefault(region => region.TwoLetterISORegionName == code);
            return englishRegion.DisplayName;
        }
    }
}
