using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace lab4.ThreadPoolModule
{
    public class CustomThreadPool : IDisposable
    {
        private readonly BlockingCollection<Action> _queue = new();
        private readonly List<Thread> _workers = new();
        private bool _disposed;

        public event EventHandler<WorkItemEventArgs>? WorkerStarted;
        public event EventHandler<WorkItemEventArgs>? WorkerStopped;
        public event EventHandler<WorkItemEventArgs>? TaskQueued;
        public event EventHandler<WorkItemEventArgs>? TaskStarted;
        public event EventHandler<WorkItemEventArgs>? TaskCompleted;
        public event EventHandler<WorkItemEventArgs>? TaskFailed;
        public CustomThreadPool(int workerCount)
        {
            for (int i = 0; i < workerCount; i++)
            {
                int workerId = i + 1;
                var thread = new Thread(() => Work(workerId))
                {
                    IsBackground = true,
                    Name = $"CustomWorker-{workerId}"
                };

                _workers.Add(thread);
                thread.Start();
            }
        }
        public void Enqueue(Action action)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(CustomThreadPool));

            _queue.Add(action);
            TaskQueued?.Invoke(this, new WorkItemEventArgs("Task added to queue", 0));
        }
        private void Work(int workerId)
        {
            WorkerStarted?.Invoke(this, new WorkItemEventArgs("Worker started", workerId));

            foreach (var action in _queue.GetConsumingEnumerable())
            {
                try
                {
                    TaskStarted?.Invoke(this, new WorkItemEventArgs("Task started", workerId));
                    action();
                    TaskCompleted?.Invoke(this, new WorkItemEventArgs("Task completed", workerId));
                }
                catch (Exception ex)
                {
                    TaskFailed?.Invoke(this, new WorkItemEventArgs($"Task failed: {ex.Message}", workerId));
                }
            }

            WorkerStopped?.Invoke(this, new WorkItemEventArgs("Worker stopped", workerId));
        }
        public void Complete()
        {
            _queue.CompleteAdding();
        }

        public void WaitAll()
        {
            foreach (var worker in _workers)
                worker.Join();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            Complete();
            WaitAll();
            _queue.Dispose();
            _disposed = true;
        }
    }

}
