using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SupplierInfoCollector.TaskFramework
{
    public class TaskExecutor
    {
        private const int STOPPED_STATUS = 0;
        private const int STOPPING_STATUS = 5;
        private const int STARTED_STATUS = 10;

        /// <summary>
        /// 0 stopped
        /// 5 stopping
        /// 10 started
        /// </summary>
        private int _status = 0;
        private readonly object _statusLock = new object();
        private readonly object _queueLock = new object();
        private readonly Queue<ITask> _taskQueue = new Queue<ITask>();
        
        public int TaskCount
        {
            get { return _taskQueue.Count; }
        }

        public void AddTask(ITask task)
        {
            lock (_queueLock)
            {
                _taskQueue.Enqueue(task);
            }
        }

        public ITask GetTask()
        {
            lock (_queueLock)
            {
                if (_taskQueue.Count > 0)
                {
                    return _taskQueue.Dequeue();
                }
                else
                {
                    return null;
                }
            }
        }

        public void Start()
        {
            lock (_statusLock)
            {
                if (_status == STOPPED_STATUS)
                {
                    _status = STARTED_STATUS;
                    ExecuteTask();
                }
            }
        }

        public void Stop()
        {
            lock (_statusLock)
            {
                if (_status == STARTED_STATUS)
                {
                    _status = STOPPING_STATUS;
                }
            }
        }

        private void ExecuteTask()
        {
            new Thread(() =>
            {
                while (true)
                {
                    if (_status == STOPPING_STATUS)
                    {
                        lock (_statusLock)
                        {
                            if (_status == STOPPING_STATUS)
                            {
                                Console.WriteLine("Stop task executor...");
                                _status = STOPPED_STATUS;
                                break;
                            }
                        }
                    }

                    ITask task = this.GetTask();
                    if (task != null)
                    {
                        try
                        {
                            Console.WriteLine(string.Format("[{0}] Start execute task",
                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                            TastExecuteResult result = task.Execute();

                            string msg = string.Format("Result: {0}, Message: {1}", result.Result, result.Message);
                            Console.WriteLine(msg);

                            Console.WriteLine(string.Format("[{0}] complete task, wait...", 
                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                            Thread.Sleep(1000);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(string.Format("任务执行失败，失败信息：{0}，堆栈信息：{1}", 
                                ex.Message, ex.StackTrace));
                        }
                    }
                    else
                    {
                        Console.WriteLine(string.Format("[{0}] There isn't any task, wait...", 
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                        Thread.Sleep(1000);
                    }
                }
            }).Start();
        }
    }
}
