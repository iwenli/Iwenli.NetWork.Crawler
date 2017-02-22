using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iwenli.NetWork.Crawler.Method
{
    /// <summary>
    /// 详情页任务列表
    /// </summary>
    class PageTask
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 下载目标
        /// </summary>
        public string Root { get; set; }

        /// <summary>
        /// 包含的图片信息
        /// </summary>
        public Dictionary<string, ImageDownloadTaskInfo> Images { get; set; }

        public PageTask(string name, string url) : this()
        {
            Name = Utility.RemoveInvalidCharacters(name);
            Url = url;
        }

        public PageTask()
        {
            Images = new Dictionary<string, ImageDownloadTaskInfo>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
