using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Iwenli.NetWork.Crawler.Method
{
    /// <summary>
    /// 将所有的任务上下文抽象成一个单例对象，保存着所有的任务和相关设置。它负责啥事儿呢？
    /// 维护自己的一个单例对象
    /// 维护全局公用的上下文任务数据仓库
    /// 负责数据的存储和读取
    /// </summary>
    class TaskContext
    {
        /// <summary>
        /// 数据目录
        /// </summary>
        public string DataRoot { get; private set; }

        /// <summary>
        /// 下载目录
        /// </summary>
        public string OutputRoot { get; set; }

        /// <summary>
        /// 任务数据
        /// </summary>
        public TaskData Data { get; private set; }

        #region 单例模式

        static TaskContext _instance;
        static readonly object _lockObject = new object();

        public static TaskContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new TaskContext();
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion

        private TaskContext()
        {

        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            var root = System.Reflection.Assembly.GetExecutingAssembly().GetLocation();
            DataRoot = PathUtility.Combine(root, "data");
            OutputRoot = PathUtility.Combine(root, "下载");

            Directory.CreateDirectory(DataRoot);
            Directory.CreateDirectory(OutputRoot);

            Data = LoadData<TaskData>("tasks.dat");
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public void Save()
        {
            SaveData(Data, "tasks.dat");
        }


        /// <summary>
        /// 加载数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        T LoadData<T>(string path)where T : class, new()
        {
            var file = PathUtility.Combine(DataRoot, path);
            if (File.Exists(file))
            {
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
            }
            return new T();
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="path"></param>
        void SaveData<T>(T data, string path)
        {
            var file = PathUtility.Combine(DataRoot, path);
            Directory.CreateDirectory(Path.GetDirectoryName(file));

            if (data == null)
            {
                File.Delete(file);
            }
            else
                File.WriteAllText(file, JsonConvert.SerializeObject(data));

        }
    }
}
