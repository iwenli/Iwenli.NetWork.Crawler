using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FSLib.Network.Http;
using Iwenli.NetWork.Crawler.Method;

namespace Iwenli.NetWork.Crawler
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 设置是否允许关闭的标记位
        /// </summary>
        bool _shutdownFlag = false;

        public Form1()
        {
            InitializeComponent();
            btnGo.Click += (s, e) =>
            {
                btnGo.Enabled = false;
                RunTask();
            };
        }



        //http://www.mmjpg.com/


        /// <summary>
        /// 开始抓取详情页
        /// </summary>
        async void GrabDetailPages(CancellationToken token)
        {
            AppendLog("[页面列表] 正在加载数据....");

            //从第一页开始...
            var page = 2;
            //var urlformat = "http://sexy.faceks.com/?page={0}";
            var urlformat = "http://www.mmjpg.com/home/{0}";
            //网络客户端
            var client = new HttpClient();
            var data = TaskContext.Instance.Data;

            while (!token.IsCancellationRequested)
            {
                AppendLog("[页面列表] 正在加载第 {0} 页", page);

                var ctx = client.Create<string>(HttpMethod.Get, urlformat.FormatWith(page));
                await ctx.SendAsync();
                if (!ctx.IsValid())
                {
                    AppendLog("[页面列表] 第 {0} 页下载失败，稍后重试", page);
                    await Task.Delay(new TimeSpan(0, 0, 10));
                }
                else
                {
                    //下载成功，获得列表
                    var matches = Regex.Matches(ctx.Result, @"span\sclass=['""]title['""].*?href=['""]([^['""]+)['""].*?_blank['""]>([^<]+)<", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    //新的任务
                    var newTasks = matches.Cast<Match>()
                                        .Select(s => new PageTask(s.Groups[2].Value, s.Groups[1].Value))
                                        .Where(s => !data.PageDownloaded.ContainsKey(s.Url) && !data.WaitForDownloadPageTasks.Any(_ => _.Url == s.Url))
                                        .ToArray();
                    if (newTasks.Length > 0)
                    {
                        lock (data.WaitForDownloadPageTasks)
                        {
                            newTasks.ForEach(s =>
                            {
                                data.WaitForDownloadPageTasks.Enqueue(s);
                            });
                        }

                        AppendLog("[页面列表] 已建立 {0} 新任务到队列中...", newTasks.Length);
                        UpdatePageDetailGrabStatus();
                    }
                    else if (data.FullyDownloaded)
                    {
                        //没有更多记录，退出循环
                        AppendLog("[页面列表] 没有更多新纪录，退出抓取...");
                        break;
                    }
                    //如果没有下一页，则中止
                    if (!Regex.IsMatch(ctx.Result, @"<a[^>]*?class=[""']ch[""']>下一页</a>", RegexOptions.IgnoreCase))
                    {
                        AppendLog("[页面列表] 没有更多的页面，退出抓取...");
                        data.FullyDownloaded = true;
                        break;
                    }

                    //等待2秒继续
                    await Task.Delay(new TimeSpan(0, 0, 2));
                    page++;
                }
            }
            AppendLog("[页面列表] 页面任务抓取完成...");
        }


        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void AppendLog(string message, params object[] args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    AppendLog(message, args);
                }));
                return;
            }
            string timeL = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            txtLog.AppendText(timeL + " => ");
            txtLog.AppendText(Environment.NewLine);

            if (args == null || args.Length == 0)
            {
                txtLog.AppendText(message);
            }
            else
            {
                txtLog.AppendText(string.Format(message, args));
            }
            txtLog.AppendText(Environment.NewLine);
            txtLog.ScrollToCaret();
        }


        /// <summary>
        /// 更新页面抓取进度
        /// </summary>
        void UpdatePageDetailGrabStatus()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdatePageDetailGrabStatus));
                return;
            }

            var data = TaskContext.Instance.Data;
            pgPage.Maximum = data.WaitForDownloadPageTasks.Count + data.PageDownloaded.Count;
            pgPage.Value = pgPage.Maximum - data.WaitForDownloadPageTasks.Count;
            lblPgSt.Text = string.Format("共 {0} 页面，已抓取 {1} 页面 ...", pgPage.Maximum, pgPage.Value);
        }

        /// <summary>
        /// 详情下载任务
        /// </summary>
        /// <param name="token"></param>
        void GrabImageListTaskThreadEntry(CancellationToken token)
        {
            var client = new HttpClient();
            var data = TaskContext.Instance.Data;

            PageTask currentTask;

            //token是用来控制队列退出的
            while (!token.IsCancellationRequested)
            {
                currentTask = null;

                //对队列进行加锁，防止详情页爬虫意外修改队列
                lock (data.WaitForDownloadPageTasks)
                {
                    //如果有任务，则出队
                    if (data.WaitForDownloadPageTasks.Count > 0)
                    {
                        currentTask = data.WaitForDownloadPageTasks.Dequeue();
                    }
                }
                //如果没有任务，则等待100毫秒后继续查询任务
                if (currentTask == null)
                {
                    Thread.Sleep(100);
                    continue;
                }

                AppendLog("[详情页抓取] 正在抓取页面 【{0}】 ...", currentTask.Name);

                //防止该死的标题过长……这个是后来加的，因为后拉发现有的标题居然长到让文件系统报错了……
                currentTask.Root = currentTask.Name.GetSubString(40);

                //创建上下文。注意 allowAutoRedirect，因为这里可能会存在重定向，而我们并不关心不是302.
                var ctx = client.Create<string>(HttpMethod.Get, currentTask.Url, allowAutoRedirect: true);

                //这里用的是同步模式，非任务模式。这个方法会阻塞当前线程直到请求结束
                ctx.Send();

                if (ctx.IsValid())
                { 
                    //页面有效
                    var html = ctx.Result;

                    ////查出所有姑娘的地址，然后与已下载和已入列的对比，排除重复后将其加入下载队列
                    //var imgTasks = Regex.Matches(html, "bigimgsrc=[\"']([^'\"]+)['\"]", RegexOptions.IgnoreCase | RegexOptions.Singleline)
                    //                    .Cast<Match>().Select(s => new ImageDownloadTask(s.Groups[1].Value, currentTask.Root))
                    //                    .Where(s =>
                    //                    {
                    //                        var ret = !data.DownloadedImages.ContainsKey(s.Url) && !data.ImageDownloadTasks.Any(_ => _.Url == s.Url);
                    //                        if (!ret)
                    //                        {
                    //                            AppendLog("[详情页抓取] 图片地址 {0} 已加入队列过，此次跳过.", s);
                    //                        }
                    //                        return ret;
                    //                    })
                    //                    .ToArray();


                    var regResult = Regex.Match(html, @"id=[""']content['""]>.*?<img src=['""]([^['""]+)['""].*?alt=['""]([^['""]+)['""]", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    if (regResult.Groups.Count < 0) return;
                    string imgUrl = regResult.Groups[1].Value;
                    string imgAlt = regResult.Groups[2].Value;

                    var imgCountRegResult = Regex.Match(html, @"<i></i><a href=[""'].*?>([\d]+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    if (imgCountRegResult.Groups.Count < 0) return;
                    int imgCount = Convert.ToInt32(imgCountRegResult.Groups[1].Value);

                    //拼接图片下载地址
                    string newImgDownloadUrl = imgUrl.Substring(0, imgUrl.LastIndexOf("/") + 1) + "{0}" + imgUrl.Remove(0, imgUrl.LastIndexOf("."));

                    List<ImageDownloadTask> imgTaskList = new List<ImageDownloadTask>();
                    for (int i = 1; i <= imgCount; i++)
                    {
                        string _imgUrl = string.Format(newImgDownloadUrl, i.ToString());
                        ImageDownloadTask s = new ImageDownloadTask(_imgUrl, currentTask.Root);
                        var ret = !data.DownloadedImages.ContainsKey(s.Url) && !data.ImageDownloadTasks.Any(_ => _.Url == s.Url);
                        if (!ret)
                        {
                            AppendLog("[详情页抓取] 图片地址 {0} 已加入队列过，此次跳过.", s);
                        }
                        else
                        {
                            imgTaskList.Add(s);
                        }
                    }
                    //var imgTasks = imgTaskList.ToArray();

                    if (imgTaskList.Count > 0)
                    {
                        lock (data.ImageDownloadTasks)
                        {
                            imgTaskList.ForEach(task =>
                            {
                                data.ImageDownloadTasks.Enqueue(task);
                            });
                        }
                        UpdateImageDownloadStatus();
                        AppendLog("[详情页抓取] 从页面 【{0}】中获得 {1} 图片地址到任务列表 ...", currentTask.Url, imgTaskList.Count);
                    }
                    else
                    {
                        AppendLog("[详情页抓取] 从页面 【{0}】中未获得任何图片地址，请检查是否正常 ...", currentTask.Url);
                    }
                    data.PageDownloaded.Add(currentTask.Url, currentTask);
                    UpdatePageDetailGrabStatus();
                }
                else
                {
                    //不成功，则将当前任务重新入队后，继续处理
                    lock (data.WaitForDownloadPageTasks)
                    {
                        data.WaitForDownloadPageTasks.Enqueue(currentTask);
                    }
                    UpdatePageDetailGrabStatus();
                    Thread.Sleep(2000);
                    AppendLog("[详情页抓取] 页面抓取失败，重新入队等待处理。");
                }
            }
        }

        /// <summary>
        /// 更新图片抓取进度
        /// </summary>
        void UpdateImageDownloadStatus()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateImageDownloadStatus));
                return;
            }

            var data = TaskContext.Instance.Data;
            pgPicDownload.Maximum = data.DownloadedImages.Count + data.ImageDownloadTasks.Count;
            pgPicDownload.Value = pgPicDownload.Maximum - data.ImageDownloadTasks.Count;
            lblStatPicDownload.Text = string.Format("共 {0} 图片，已抓取 {1} 图片 ...", pgPicDownload.Maximum, pgPicDownload.Value);
        }

        /// <summary>
        /// 图片下载任务
        /// </summary>
        /// <param name="token"></param>
        async void DownloadImageTaskEntry(CancellationToken token)
        {
            var client = new HttpClient();
            var data = TaskContext.Instance.Data;
            var random = new Random();
            var cleanupcount = 0;

            //这里创建了一个 CancellationTokenSource 的局部变量，主要是为了在循环中对请求也能进行中断
            CancellationTokenSource tcs = null;
            token.Register(() => tcs?.Cancel());

            ImageDownloadTask task;

            while (!token.IsCancellationRequested)
            {
                task = null;

                //检查下载队列，看是否有姑娘的地址搭讪到了……
                lock (data.ImageDownloadTasks)
                {
                    if (data.ImageDownloadTasks.Count > 0)
                    {
                        task = data.ImageDownloadTasks.Dequeue();
                    }
                }
                //没有或已经下载过的话，则休息后重新检查
                if (task == null || data.DownloadedImages.ContainsKey(task.Url))
                {
                    Thread.Sleep(100);
                    continue;
                }

                //开始下载
                AppendLog("[图片下载] 正在下载自 {0} ...", task.Url);

                using (var ctx = client.Create<byte[]>(HttpMethod.Get, task.Url))
                {
                    //这里的token必须用新的，否则会导致内存短期内无法释放，内存暴涨
                    tcs = new CancellationTokenSource();
                    await ctx.SendAsync(tcs.Token);
                    tcs = null;

                    if (ctx.IsValid())
                    {
                        //成功，保存。优先取URL地址中的文件名作为保存的文件名
                        var targetFileName = new Uri(task.Url).Segments.LastOrDefault();

                        //如果文件名不合法，则重新生成随机的文件名
                        if (targetFileName.IsNullOrEmpty() || targetFileName.Length > 50 || Path.GetInvalidFileNameChars().Any(s => targetFileName.Contains(s)))
                        {
                            //包含无效的文件名，则重新生成随机的
                            targetFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + random.Next(int.MaxValue) + ".jpg";
                        }

                        //....这个if是冗余的，因为重构前任务的保存地址被设置成绝对地址了。但是后来两台机器之间转移时发现下载就错位了
                        //所以后来改成保存相对地址。但是之前的任务文件懒得手动改，所以加个冗余做检测
                         
                        if (Path.IsPathRooted(task.DownloadRoot))
                        {
                            var dirName = Path.GetFileName(task.DownloadRoot);
                            if (dirName.Length > 40)
                                task.DownloadRoot = PathUtility.Combine(Path.GetDirectoryName(task.DownloadRoot), dirName.Substring(0, 40));

                        }
                        else
                        {
                            //需要判断长度是因为之前没判断，结果他喵的有超长的……
                            task.DownloadRoot = task.DownloadRoot.Length > 40 ? task.DownloadRoot.Substring(0, 40) : task.DownloadRoot;
                        }

                        task.DownloadRoot = TaskContext.Instance.OutputRoot + "//" + task.DownloadRoot;
                        //如果文件夹不存在，则创建 
                        if (!Directory.Exists(task.DownloadRoot))
                            Directory.CreateDirectory(task.DownloadRoot);

                        //写入文件
                        var targetFullPath = PathUtility.Combine(task.DownloadRoot, targetFileName);
                        File.WriteAllBytes(targetFullPath, ctx.Result);
                        //添加到已完成队列
                        data.DownloadedImages.Add(task.Url, new ImageDownloadTaskInfo() { Location = targetFullPath });

                        //记录
                        AppendLog("[图片下载] 下载成功. ({0})", ctx.Result.Length.ToString());
                    }
                    else
                    {
                        lock (data.ImageDownloadTasks)
                        {
                            data.ImageDownloadTasks.Enqueue(task);
                        }
                        AppendLog("[图片下载] 下载失败。重新加入队列以便于重新下载");
                    }
                }
                UpdateImageDownloadStatus();

                //等待一秒再下下一个姑娘，虽然精虫上脑但咱是有尊严的码农！
                tcs = new CancellationTokenSource(); 
                await Task.Delay(1000, tcs.Token);
                tcs = null;

                if (cleanupcount++ > 20)
                {
                    //每下载20个美女后手动释放一下内存
                    cleanupcount = 0;
                    GC.Collect();
                    //保存任务数据，防止什么时候宕机了任务进度回滚太多
                    TaskContext.Instance.Save();
                }
            }
        }

        void RunTask()
        {
            btnGo.Enabled = false;

            var cts = new CancellationTokenSource();

            AppendLog("[全局] 正在初始化...");
            TaskContext.Instance.Init();
            AppendLog("[全局] 初始化完成...");

            AppendLog("[全局] 启动图片下载任务...");
            var imgDownloadToken = new Task(() => DownloadImageTaskEntry(cts.Token), cts.Token, TaskCreationOptions.LongRunning);
            imgDownloadToken.Start();
            AppendLog("[全局] 图片下载任务已启动...");

            AppendLog("[全局] 启动详情页下载任务...");
            var pageDownloadTask = new Task(() => GrabImageListTaskThreadEntry(cts.Token), cts.Token, TaskCreationOptions.LongRunning);
            pageDownloadTask.Start();
            AppendLog("[全局] 详情页下载任务已启动...");

            AppendLog("[全局] 启动详情页抓取任务...");
            var detailPageGrab = new Task(() => GrabDetailPages(cts.Token), cts.Token, TaskCreationOptions.LongRunning);
            detailPageGrab.Start();
            AppendLog("[全局] 详情页抓取任务已启动...");

            //捕捉窗口关闭事件
            //主要是给一个机会等待任务完成并把任务数据都保存
            FormClosing += async (s, e) =>
            {
                if (_shutdownFlag)
                    return;

                e.Cancel = !_shutdownFlag;
                AppendLog("[全局] 等待任务结束...");
                cts.Cancel();
                try
                {
                    await detailPageGrab;
                    await pageDownloadTask;
                    await imgDownloadToken;
                }
                catch (Exception ex)
                {

                }
                _shutdownFlag = true;
                TaskContext.Instance.Save();
                Close();
            };
        }
    }
}
