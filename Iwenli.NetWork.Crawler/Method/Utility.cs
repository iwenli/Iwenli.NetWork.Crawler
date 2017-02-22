using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Iwenli.NetWork.Crawler.Method
{
    class Utility
    {
        private static Regex _removeInvalidCharReg = new Regex("[" + Path.GetInvalidFileNameChars().JoinAsString("").Replace(@"\", @"\\") + "]");

        /// <summary>
        /// 移除路径中无效的字符
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string RemoveInvalidCharacters(string name)
        {
            if (String.IsNullOrEmpty(name))
                return name;

            name = HttpUtility.HtmlDecode(name);
            name = Regex.Replace(name, @"(^\s+|[\r\n]|\s+$)", "");
            return _removeInvalidCharReg.Replace(name, "_");
        }
    }
}
