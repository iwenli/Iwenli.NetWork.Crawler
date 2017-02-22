using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iwenli.NetWork.Crawler.Method
{
    /// <summary>
    /// 图片下载任务
    /// </summary>
    class ImageDownloadTask
    {
        /// <summary>
        /// 图片地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 下载目标
        /// </summary>
        public string DownloadRoot { get; set; }

        public ImageDownloadTask(string url, string root)
        {
            DownloadRoot = root;
            Url = url;
        }

        public ImageDownloadTask()
        {

        }
    }
}
