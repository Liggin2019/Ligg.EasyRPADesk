using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Ligg.Infrastructure.Base.DataModel.Enums;
using Ligg.Infrastructure.Base.Extension;
using Ligg.Infrastructure.Base.Helpers;

namespace Ligg.Infrastructure.Base.Handlers
{
    public class ManagedThreadPool
    {
        public delegate Object WorkDelegate(Object paramObj);

        private readonly int _workerNum;
        private readonly BackgroundWorker[] _workers;
        private readonly Object _threadLock;

        private readonly List<ThreadTask> _tasks;
        public List<ThreadTask> Tasks { get { return _tasks; } }

        public event EventHandler OnTaskJoin;
        public event EventHandler OnTaskStart;
        public event EventHandler OnTaskBreak;
        public event EventHandler OnTaskComplete;
        public event EventHandler OnAllTasksComplete;

        public ManagedThreadPool(int threadNum)
        {
            _workerNum = threadNum;
            _workers = new BackgroundWorker[threadNum];
            int i;
            for (i = 0; i < threadNum; i++)
            {
                _workers[i] = new BackgroundWorker();
                _workers[i].DoWork += new DoWorkEventHandler(DoWork);
                _workers[i].RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);
                _workers[i].WorkerSupportsCancellation = true;

            }
            _tasks = new List<ThreadTask>();
            _threadLock = new Object();
        }

        //#proc
        public List<ThreadTaskInfo> GetThreadPoolInfo()
        {

            var taskInfos = new List<ThreadTaskInfo>();
            foreach (var task in _tasks)
            {
                var taskInfo = new ThreadTaskInfo();
                taskInfo.Id = task.Id;
                taskInfo.DisplayName = task.DisplayName;
                taskInfo.StatusName = task.Status.ToString();

                taskInfo.JoinTime = task.JoinTime;
                taskInfo.StartTime = task.StartTime;
                taskInfo.CompleteTime = task.CompleteTime;

                taskInfos.Add(taskInfo);
            }
            return taskInfos;

        }

        public string Join(WorkDelegate workDelegate, string displayName, Object transactionParams)
        {
            lock (_threadLock)
            {
                var id = "".ToUniqueStringByShortGuid(null);
                var task = new ThreadTask(workDelegate, id, displayName, transactionParams);
                task.JoinTime = SystemTimeHelper.Now();
                task.Status = TaskStatus.Waiting;
                _tasks.Add(task);

                if (OnTaskJoin != null)
                {
                    var args = new TaskEventArgs(id, displayName, transactionParams);
                    OnTaskJoin(this, args);
                }

                int i;
                bool isWorkerPoolBusy = true;
                for (i = 0; i < _workerNum; i++)
                {
                    if (!_workers[i].IsBusy)
                    {
                        task.WorkerId = i;
                        task.Status = TaskStatus.Processing;
                        RaiseWork(_workers[i], task);
                        isWorkerPoolBusy = false;
                        break;
                    }
                }
                if (isWorkerPoolBusy)
                {
                    task.Status = TaskStatus.Waiting;
                }
                return id;
            }
        }

        private void RaiseWork(BackgroundWorker worker, ThreadTask task)
        {
            task.Status = TaskStatus.Processing;
            task.StartTime = SystemTimeHelper.Now();
            worker.RunWorkerAsync(task);
            if (OnTaskStart != null)
            {
                OnTaskStart(this, new TaskEventArgs(task.Id, task.DisplayName, task.TransactionParamsObj));
            }
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            var task = (ThreadTask)(e.Argument);
            e.Result = new TaskCompletedResult(task.Id, task.WorkerId, task.Work.Invoke(task.TransactionParamsObj));
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock (_threadLock)
            {
                var result = (TaskCompletedResult)(e.Result);
                var completedTask = _tasks.Find(x => x.Id == result.TaskId);
                completedTask.Status = TaskStatus.Completed;
                completedTask.CompleteTime = SystemTimeHelper.Now();
                if (OnTaskComplete != null)
                {
                    OnTaskComplete(this, new TaskResultEventArgs(result.TaskId, completedTask.DisplayName, completedTask.TransactionParamsObj, result.ResultObj));
                }

                var toBeProcessedTasks = _tasks.FindAll(x => x.Status == TaskStatus.Waiting);
                if (toBeProcessedTasks.Count > 0)
                {
                    var worker = (BackgroundWorker)(sender);
                    if (worker.IsBusy == false)
                    {
                        var toBeProcessedTask = toBeProcessedTasks.FirstOrDefault();
                        if (toBeProcessedTask != null)
                        {
                            toBeProcessedTask.WorkerId = result.WorkerId;
                            RaiseWork(worker, toBeProcessedTask);
                        }
                    }
                }

                var waitingOrProcessingTasks = _tasks.FindAll(x => x.Status == TaskStatus.Waiting | x.Status == TaskStatus.Processing);
                if (waitingOrProcessingTasks.Count == 0)
                {
                    if (OnAllTasksComplete != null)
                    {
                        OnAllTasksComplete(this, new EventArgs());
                    }
                }
            }
        }


        //#internal class
        public class ThreadTask
        {
            public string Id { get; private set; }
            public int WorkerId { get; set; }
            public string DisplayName { get; set; }
            public string FailureReason { get; set; }

            public TaskStatus Status { get; set; }
            public DateTime? JoinTime { get; set; }
            public DateTime? StartTime { get; set; }
            public DateTime? CompleteTime { get; set; }

            public Object TransactionParamsObj { get; private set; }
            public readonly WorkDelegate Work;
            public ThreadTask(WorkDelegate work, string id, string displayName, Object transactionParamsObj)
            {
                Id = id;
                DisplayName = displayName;
                Work = work;
                TransactionParamsObj = transactionParamsObj;
            }
        }

        private class TaskCompletedResult
        {
            public string TaskId { get; private set; }
            public int WorkerId { get; set; }
            public Object ResultObj { get; private set; }
            public TaskCompletedResult(string taskId, int workerId, Object resultObj)
            {
                ResultObj = resultObj;
                TaskId = taskId;
                WorkerId = workerId;
            }
        }


        private class TaskEventArgs : EventArgs
        {
            public string Id { get; private set; }
            public string DispalyName { get; private set; }
            public Object TransactionParamsObj { get; private set; }

            public TaskEventArgs(string id, string displayName, object param)
            {
                Id = id;
                DispalyName = displayName;
                TransactionParamsObj = param;
            }
        }

        private class TaskResultEventArgs : EventArgs
        {
            public string TaskId { get; private set; }
            public string DispalyName { get; private set; }
            public Object TransactionParamsObj { get; private set; }
            public Object ResultObj { get; private set; }
            public TaskResultEventArgs(string taskId, string displayName, object transactionParamsObj, object resultObj)
            {
                TaskId = taskId;
                DispalyName = displayName;
                TransactionParamsObj = transactionParamsObj;
                ResultObj = resultObj;
            }
        }

        public enum TaskStatus
        {
            Waiting = 0,
            Processing = 1,
            Completed = 3,
            Suspended = 4,
            Breaked = 5,
        }

    }

    public class ThreadTaskInfo
    {
        public string Id;
        public string DisplayName;
        public string StatusName;
        public DateTime? JoinTime;
        public DateTime? StartTime;
        public DateTime? CompleteTime;
    }
}
