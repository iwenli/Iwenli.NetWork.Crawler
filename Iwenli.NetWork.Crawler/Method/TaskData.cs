using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iwenli.NetWork.Crawler.Method
{
    /// <summary>
    /// 数据存储类
    /// </summary>
    class TaskData
    {
        /// <summary>
        /// 已下载的页面列表
        /// </summary>
        public Dictionary<string, PageTask> PageDownloaded { get; set; }

        /// <summary>
        /// 等待下载的页面队列任务
        /// </summary>
        public Queue<PageTask> WaitForDownloadPageTasks { get; set; }

        /// <summary>
        /// 已经下载的图片列表
        /// </summary>
        public Dictionary<string, ImageDownloadTaskInfo> DownloadedImages { get; set; }

        /// <summary>
        /// 准备下载的图片列表
        /// </summary>
        public Queue<ImageDownloadTask> ImageDownloadTasks { get; set; }

        /// <summary>
        /// 获得或设置是否已经完整下载过
        /// </summary>
        public bool FullyDownloaded { get; set; }

        public TaskData()
        {
            PageDownloaded = new Dictionary<string, PageTask>(StringComparer.OrdinalIgnoreCase);
            DownloadedImages = new Dictionary<string, ImageDownloadTaskInfo>(StringComparer.OrdinalIgnoreCase);
            WaitForDownloadPageTasks = new Queue<PageTask>();
            ImageDownloadTasks = new Queue<ImageDownloadTask>();
        }
    }
}
