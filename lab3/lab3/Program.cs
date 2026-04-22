using DynamicThreadPoolLib;
using lab3.Tests;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace lab3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Dynamic Custom Thread Pool Test Runner ===");

            using var pool = new DynamicThreadPool(
                minThreads: 2,
                maxThreads: 6,
                idleTimeout: TimeSpan.FromSeconds(4),
                queueWaitThreshold: TimeSpan.FromSeconds(1)
            );

            int total = 0;
            int passed = 0;
            int failed = 0;

            object resultLock = new object();

            var testsAssembly = typeof(Testing.PrTests).Assembly;

            var jobs = TestRunner.BuildTestsFromAssembly(testsAssembly, result =>
            {
                lock (resultLock)
                {
                    total++;
                    if (result.passed) passed++;
                    else failed++;

                    Console.WriteLine(
                        $"[RESULT] {result.className}.{result.methodName} | " +
                        $"{(result.passed ? "PASS" : "FAIL")} | " +
                        $"{result.durationMs} ms | {result.message}");
                }
            });

            Console.WriteLine($"Discovered jobs: {jobs.Count}");

            int launchesNeeded = 50;
            int launched = 0;

            while (launched < launchesNeeded)
            {
                // 1) одиночные подачи
                for (int i = 0; i < 5 && launched < launchesNeeded; i++)
                {
                    var job = jobs[launched % jobs.Count];
                    pool.Enqueue(job);
                    launched++;
                    Thread.Sleep(300);
                }

                // 2) период бездействия
                Console.WriteLine("\n=== Idle period ===\n");
                Thread.Sleep(5000);

                // 3) пиковая нагрузка
                Console.WriteLine("\n=== Peak load ===\n");
                for (int i = 0; i < 20 && launched < launchesNeeded; i++)
                {
                    var job = jobs[launched % jobs.Count];
                    pool.Enqueue(job);
                    launched++;
                }

                Thread.Sleep(1500);

                // 4) снова одиночные
                Console.WriteLine("\n=== Single arrivals ===\n");
                for (int i = 0; i < 10 && launched < launchesNeeded; i++)
                {
                    var job = jobs[launched % jobs.Count];
                    pool.Enqueue(job);
                    launched++;
                    Thread.Sleep(500);
                }
            }

            pool.WaitAll();

            Console.WriteLine("\n=== FINAL STATS ===");
            Console.WriteLine($"Total launches: {total}");
            Console.WriteLine($"Passed: {passed}");
            Console.WriteLine($"Failed: {failed}");
            Console.WriteLine("Done.");
        }
    }
}
