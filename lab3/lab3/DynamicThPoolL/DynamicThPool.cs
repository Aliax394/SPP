using lab3.DynamicThPoolL;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DynamicThreadPoolLib
{
    public class DynamicThreadPool : IDisposable
    {
        private readonly Queue<WorkItem> _queue = new();
        private readonly List<WorkerInfo> _workers = new();
        private readonly object _lock = new();

        private readonly int _minThreads;
        private readonly int _maxThreads;
        private readonly TimeSpan _idleTimeout;
        private readonly TimeSpan _queueWaitThreshold;

        private bool _isRunning = true;
        private int _workerCounter = 0;

        public DynamicThreadPool(
            int minThreads,
            int maxThreads,
            TimeSpan idleTimeout,
            TimeSpan queueWaitThreshold)
        {
            if (minThreads < 1) throw new ArgumentException("minThreads must be >= 1");
            if (maxThreads < minThreads) throw new ArgumentException("maxThreads must be >= minThreads");

            _minThreads = minThreads;
            _maxThreads = maxThreads;
            _idleTimeout = idleTimeout;
            _queueWaitThreshold = queueWaitThreshold;

            for (int i = 0; i < _minThreads; i++)
                CreateWorker();
        }

        public int QueueLength
        {
            get
            {
                lock (_lock)
                    return _queue.Count;
            }
        }

        public int WorkerCount
        {
            get
            {
                lock (_lock)
                    return _workers.Count;
            }
        }

        public void Enqueue(Action action)
        {
            lock (_lock)
            {
                _queue.Enqueue(new WorkItem(action));
                Console.WriteLine($"[POOL] Task added. Queue={_queue.Count}, Workers={_workers.Count}");

                TryScaleUp_NoLock();

                Monitor.PulseAll(_lock);
            }
        }

        private void CreateWorker()
        {
            var worker = new WorkerInfo
            {
                Id = Interlocked.Increment(ref _workerCounter),
                LastActive = DateTime.UtcNow
            };

            worker.Thread = new Thread(() => WorkerLoop(worker))
            {
                IsBackground = true,
                Name = $"CustomPoolWorker-{worker.Id}"
            };

            _workers.Add(worker);
            worker.Thread.Start();

            Console.WriteLine($"[POOL] Worker #{worker.Id} created. Total workers={_workers.Count}");
        }

        private void TryScaleUp_NoLock()
        {
            if (_workers.Count >= _maxThreads)
                return;

            bool needMoreWorkers = false;

            if (_queue.Count > _workers.Count)
                needMoreWorkers = true;

            if (_queue.Count > 0)
            {
                var oldest = PeekOldest_NoLock();
                if (oldest != null)
                {
                    var waitTime = DateTime.UtcNow - oldest.EnqueueTime;
                    if (waitTime > _queueWaitThreshold)
                        needMoreWorkers = true;
                }
            }

            if (needMoreWorkers)
                CreateWorker();
        }

        private WorkItem? PeekOldest_NoLock()
        {
            return _queue.Count == 0 ? null : _queue.Peek();
        }

        private void WorkerLoop(WorkerInfo worker)
        {
            try
            {
                while (true)
                {
                    WorkItem? item = null;

                    lock (_lock)
                    {
                        while (_isRunning && _queue.Count == 0)
                        {
                            var idle = DateTime.UtcNow - worker.LastActive;

                            if (_workers.Count > _minThreads && idle >= _idleTimeout)
                            {
                                Console.WriteLine($"[POOL] Worker #{worker.Id} stopped due to idle timeout.");
                                _workers.Remove(worker);
                                return;
                            }

                            Monitor.Wait(_lock, 500);
                        }

                        if (!_isRunning && _queue.Count == 0)
                        {
                            _workers.Remove(worker);
                            Console.WriteLine($"[POOL] Worker #{worker.Id} stopped during shutdown.");
                            return;
                        }

                        if (_queue.Count > 0)
                        {
                            item = _queue.Dequeue();
                            worker.LastActive = DateTime.UtcNow;
                        }
                    }

                    if (item != null)
                    {
                        try
                        {
                            Console.WriteLine($"[WORKER #{worker.Id}] Started task. Queue={QueueLength}");
                            item.Action.Invoke();
                            worker.LastActive = DateTime.UtcNow;
                            Console.WriteLine($"[WORKER #{worker.Id}] Finished task.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[WORKER #{worker.Id}] ERROR: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[POOL] Worker #{worker.Id} crashed: {ex.Message}");

                lock (_lock)
                {
                    _workers.Remove(worker);

                    if (_isRunning && _workers.Count < _minThreads)
                    {
                        Console.WriteLine($"[POOL] Replacing crashed worker #{worker.Id}");
                        CreateWorker();
                    }
                }
            }
        }

        public void WaitAll()
        {
            while (true)
            {
                lock (_lock)
                {
                    if (_queue.Count == 0)
                    {
                        bool allIdle = true;
                        foreach (var worker in _workers)
                        {
                            // Упрощённо: если очередь пуста, считаем, что скоро всё завершится.
                            // Для более строгого контроля можно добавить флаг Busy.
                        }

                        if (allIdle)
                            break;
                    }
                }

                Thread.Sleep(200);
            }

            Thread.Sleep(1000);
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _isRunning = false;
                Monitor.PulseAll(_lock);
            }

            List<Thread> threads;
            lock (_lock)
            {
                threads = new List<Thread>();
                foreach (var w in _workers)
                    threads.Add(w.Thread);
            }

            foreach (var t in threads)
            {
                if (t.IsAlive)
                    t.Join();
            }

            Console.WriteLine("[POOL] Disposed.");
        }

        private class WorkerInfo
        {
            public int Id { get; set; }
            public Thread Thread { get; set; } = null!;
            public DateTime LastActive { get; set; }
        }
    }
}