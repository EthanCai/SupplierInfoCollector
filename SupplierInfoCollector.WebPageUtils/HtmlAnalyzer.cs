using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupplierInfoCollector.WebPageUtils
{
    public class HtmlAnalyzer
    {
        public static string CleanText(string text)
        {
            if (text == null)
            {
                return null;
            }
            return text.Replace("&nbsp;", "").Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Trim();
        }
    }
}
