using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Đồ_Án_Web.Others
{
    public class SoSanh: IComparer<string>
    {
        public int Compare(string key, string value)
        {
            if (key == value) return 0;
            if (key == null) return -1;
            if (value == null) return 1;
            var vnpCompare = CompareInfo.GetCompareInfo("en-US");
            return vnpCompare.Compare(key, value, CompareOptions.Ordinal);
        }
    }
}