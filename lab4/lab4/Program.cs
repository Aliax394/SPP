using lab4.Tests.Core;
using lab4.Tests.Filters;
using lab4.ThreadPoolModule;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;


namespace LoadAndRunApp
{
    internal class Program
    {
        static async Task Main()
        {
            Console.WriteLine("=== DEMO: custom thread pool ===");

            using (var pool = new CustomThreadPool(workerCount: 3))
            {
                pool.WorkerStarted += (_, e) => Console.WriteLine($"[EVENT] Worker {e.WorkerId}: {e.Message}");
                pool.WorkerStopped += (_, e) => Console.WriteLine($"[EVENT] Worker {e.WorkerId}: {e.Message}");
                pool.TaskQueued += (_, e) => Console.WriteLine($"[EVENT] {e.Message}");
                pool.TaskStarted += (_, e) => Console.WriteLine($"[EVENT] Worker {e.WorkerId}: {e.Message}");
                pool.TaskCompleted += (_, e) => Console.WriteLine($"[EVENT] Worker {e.WorkerId}: {e.Message}");
                pool.TaskFailed += (_, e) => Console.WriteLine($"[EVENT] Worker {e.WorkerId}: {e.Message}");

                for (int i = 1; i <= 6; i++)
                {
                    int taskId = i;
                    pool.Enqueue(() =>
                    {
                        Console.WriteLine($"Task {taskId} is running...");
                        Thread.Sleep(600);
                        if (taskId == 5)
                            throw new Exception("Artificial failure in task 5");
                        Console.WriteLine($"Task {taskId} finished.");
                    });
                }
            }
            Console.WriteLine();
            Console.WriteLine("=== DEMO: test filtering and execution ===");

            var runner = new TestRunner();
            var testsAssembly = typeof(lab4.TestsProject.TaskManagerTests).Assembly;

            var filter = FilterFactory.CombineAnd(
                FilterFactory.ByAuthor("Pavel"),
                entry => entry.Category == "Math" || entry.Category == "Tasks",
                FilterFactory.ByMinPriority(1)
            );

            var results = await runner.RunAsync(testsAssembly, filter);

            foreach (var result in results)
            {
                Console.WriteLine($"[{(result.Passed ? "PASS" : result.Skipped ? "SKIP" : "FAIL")}] " +
                                  $"{result.TestName} | {result.DurationMs} ms | {result.Message}");
            }

            Console.WriteLine();
            Console.WriteLine($"Passed: {results.Count(r => r.Passed)}");
            Console.WriteLine($"Failed: {results.Count(r => !r.Passed && !r.Skipped)}");
            Console.WriteLine($"Skipped: {results.Count(r => r.Skipped)}");
        }
    }
}
