using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Coralcode.Framework.ConfigManager;
using Coralcode.Framework.Data;
using Coralcode.Framework.Log;
using ThreadState = System.Threading.ThreadState;

namespace Coralcode.Framework.Task
{
    public class TaskQueue
    {
        public static readonly TaskQueue Instance = new TaskQueue(1);


        public static TaskQueue GetQueue(int maxCount)
        {
            return new TaskQueue(maxCount);
        }
        public static TaskQueue GetQueue(int maxCount, string name)
        {
            return new TaskQueue(maxCount, name);
        }
        public static TaskQueue GetQueue(int maxCount, string name, CancellationToken token)
        {
            return new TaskQueue(maxCount, name, token);
        }
        public static TaskQueue GetQueue(int maxCount, CancellationToken token)
        {
            return new TaskQueue(maxCount, IdentityGenerator.NewGuidString(), token);
        }

        private ConcurrentQueue<System.Threading.Tasks.Task> _tasks;
        private readonly int _limitedTaskCount;
        private int _runningTaskCount;
        private Thread _mainExcuteThread;
        private CancellationToken _token;

        private TaskQueue(int maxCount) :
            this(maxCount, IdentityGenerator.NewGuidString())
        {
        }

        private TaskQueue(int maxCount, string name) :
            this(maxCount, name, CancellationToken.None)

        {
        }

        private TaskQueue(int maxCount, string name, CancellationToken token)
        {
            _limitedTaskCount = maxCount;
            _tasks = new ConcurrentQueue<System.Threading.Tasks.Task>();
            Name = name;
            _token = token;
        }

        public System.Threading.Tasks.Task Execute(Action func)
        {
            var task = new System.Threading.Tasks.Task(func, _token);
            _tasks.Enqueue(task);
            if (_mainExcuteThread == null || _mainExcuteThread.ThreadState.HasFlag(ThreadState.Stopped))
            {
                _mainExcuteThread?.DisableComObjectEagerCleanup();
                _mainExcuteThread = new Thread(NotifyThreadPendingWork);
                _mainExcuteThread.Start();
            }
            return task;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Task<TResult> Execute<TResult>(Func<TResult> func)
        {
            var task = new Task<TResult>(func, _token);
            _tasks.Enqueue(task);
            if (_mainExcuteThread == null || _mainExcuteThread.ThreadState.HasFlag(ThreadState.Stopped))
            {
                _mainExcuteThread?.DisableComObjectEagerCleanup();
                _mainExcuteThread = new Thread(NotifyThreadPendingWork);
                _mainExcuteThread.Start();
            }
            return task;
        }

        private void NotifyThreadPendingWork()
        {
            try
            {
                while (true)
                {
                    if(_token.IsCancellationRequested) 
                    {
                        _tasks = new ConcurrentQueue<System.Threading.Tasks.Task>();
                        break;
                    }
                    
                    System.Threading.Tasks.Task task;
                    if (!_tasks.TryDequeue(out task))
                        break;
                    
                    task.Start();
                    Interlocked.Increment(ref _runningTaskCount);
                    task.ContinueWith(item =>
                    {
                        Interlocked.Decrement(ref _runningTaskCount);
                    });
                    //Debug.WriteLine("队列{0},允许执行 {1} 条,等待线程为 {2} ,执行中 {3} 条,时间为 {4} ", _name, _limitedTaskCount, _tasks.Count, _runningTaskCount, DateTime.Now);
                    while (_runningTaskCount >= _limitedTaskCount)
                    {
                        Thread.Sleep(500);
                    }
                }
            }
            finally
            {
                _runningTaskCount = 0;
            }
        }

        public string Name { get; }

        public int WaitingTaskCount => _tasks.Count;
        public int RunningTaskCount => _runningTaskCount;
        public int LimitedTaskCount => _limitedTaskCount;
    }
}
