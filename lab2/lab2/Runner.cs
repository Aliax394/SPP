using lab2.Project;
using lab2.Tests;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace lab2
{
    internal class Runner
    {
        private readonly object _consoleLock = new object();

        private int _passed;
        private int _failed;
        private int _skipped;

        public int MaxDegreeOfParallelism { get; set; } = 4;

        public void RunAllTests(Assembly assembly)
        {
            var testClasses = assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<TestClassAttr>() != null)
                .ToList();

            SafeWriteLine($"Found {testClasses.Count} test class(es).");
            SafeWriteLine("");

            var allTestEntries = new List<TestExecutionEntry>();

            foreach (var testClass in testClasses)
            {
                PrepareTestsFromClass(testClass, allTestEntries);
            }

            SafeWriteLine("===== SEQUENTIAL RUN =====");
            ResetCounters();
            var sequentialWatch = Stopwatch.StartNew();
            RunSequential(allTestEntries);
            sequentialWatch.Stop();
            PrintResults("SEQUENTIAL RESULTS", sequentialWatch.Elapsed);

            SafeWriteLine("");
            SafeWriteLine("===== PARALLEL RUN =====");
            ResetCounters();
            var parallelWatch = Stopwatch.StartNew();
            RunParallel(allTestEntries).GetAwaiter().GetResult();
            parallelWatch.Stop();
            PrintResults("PARALLEL RESULTS", parallelWatch.Elapsed);
        }

        private void PrepareTestsFromClass(Type testClassType, List<TestExecutionEntry> entries)
        {
            SafeWriteLine($"Preparing class: {testClassType.Name}");

            object? instance = null;
            if (!IsStaticClass(testClassType))
                instance = Activator.CreateInstance(testClassType);

            var methods = testClassType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            var beforeAllMethods = methods
                .Where(m => m.GetCustomAttribute<BeforeAllAttr>() != null)
                .ToList();

            var afterAllMethods = methods
                .Where(m => m.GetCustomAttribute<AfterAllAttr>() != null)
                .ToList();

            foreach (var beforeMethod in beforeAllMethods)
                InvokeLifecycleMethod(beforeMethod, instance, "BeforeAll");

            var testMethods = methods
                .Where(m => m.GetCustomAttribute<TestAttr>() != null)
                .OrderBy(m => m.GetCustomAttribute<TestAttr>()?.Priority ?? 0)
                .ToList();

            foreach (var method in testMethods)
            {
                var testAttr = method.GetCustomAttribute<TestAttr>()!;
                var infoAttr = method.GetCustomAttribute<TestInfoAttr>();
                var timeoutAttr = method.GetCustomAttribute<TimeoutAttribute>();
                var testCases = method.GetCustomAttributes<TestCaseAttr>().ToList();

                if (!testAttr.Enabled)
                {
                    entries.Add(new TestExecutionEntry
                    {
                        Method = method,
                        Instance = instance,
                        Parameters = null,
                        IsSkipped = true,
                        Priority = testAttr.Priority,
                        Author = infoAttr?.Author ?? "Unknown",
                        Category = infoAttr?.Category ?? "Unspecified",
                        TimeoutMilliseconds = timeoutAttr?.Milliseconds
                    });
                    continue;
                }

                if (testCases.Count == 0)
                {
                    entries.Add(new TestExecutionEntry
                    {
                        Method = method,
                        Instance = instance,
                        Parameters = null,
                        IsSkipped = false,
                        Priority = testAttr.Priority,
                        Author = infoAttr?.Author ?? "Unknown",
                        Category = infoAttr?.Category ?? "Unspecified",
                        TimeoutMilliseconds = timeoutAttr?.Milliseconds
                    });
                }
                else
                {
                    foreach (var testCase in testCases)
                    {
                        entries.Add(new TestExecutionEntry
                        {
                            Method = method,
                            Instance = instance,
                            Parameters = testCase.Parameters,
                            IsSkipped = false,
                            Priority = testAttr.Priority,
                            Author = infoAttr?.Author ?? "Unknown",
                            Category = infoAttr?.Category ?? "Unspecified",
                            TimeoutMilliseconds = timeoutAttr?.Milliseconds
                        });
                    }
                }
            }

            foreach (var afterMethod in afterAllMethods)
                entries.Add(new TestExecutionEntry
                {
                    LifecycleMethod = afterMethod,
                    Instance = instance,
                    IsAfterAll = true
                });

            SafeWriteLine("");
        }

        private void RunSequential(List<TestExecutionEntry> entries)
        {
            foreach (var entry in entries)
            {
                if (entry.IsAfterAll)
                {
                    InvokeLifecycleMethod(entry.LifecycleMethod!, entry.Instance, "AfterAll");
                    continue;
                }

                ExecuteTest(entry).GetAwaiter().GetResult();
            }
        }

        private async Task RunParallel(List<TestExecutionEntry> entries)
        {
            var semaphore = new SemaphoreSlim(MaxDegreeOfParallelism);
            var tasks = new List<Task>();

            foreach (var entry in entries)
            {
                if (entry.IsAfterAll)
                    continue;

                await semaphore.WaitAsync();

                var task = Task.Run(async () =>
                {
                    try
                    {
                        await ExecuteTest(entry);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            foreach (var entry in entries.Where(e => e.IsAfterAll))
            {
                InvokeLifecycleMethod(entry.LifecycleMethod!, entry.Instance, "AfterAll");
            }
        }

        private async Task ExecuteTest(TestExecutionEntry entry)
        {
            if (entry.IsSkipped)
            {
                SafeWriteLine($"[SKIPPED] {entry.Method!.Name} (Enabled = false)");
                Interlocked.Increment(ref _skipped);
                return;
            }

            string argsText = entry.Parameters == null
                ? ""
                : $" | Args: {string.Join(", ", entry.Parameters.Select(p => p?.ToString() ?? "null"))}";

            SafeWriteLine(
                $"Running: {entry.Method!.Name}{argsText} | Priority={entry.Priority} | Author={entry.Author} | Category={entry.Category}");

            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (entry.TimeoutMilliseconds.HasValue)
                {
                    await ExecuteWithTimeout(entry, entry.TimeoutMilliseconds.Value);
                }
                else
                {
                    entry.Method.Invoke(entry.Instance, entry.Parameters);
                }

                stopwatch.Stop();
                SafeWriteLine($"[PASSED] {entry.Method.Name} ({stopwatch.ElapsedMilliseconds} ms)");
                Interlocked.Increment(ref _passed);
            }
            catch (TargetInvocationException ex)
            {
                stopwatch.Stop();
                SafeWriteLine($"[FAILED] {entry.Method.Name} ({stopwatch.ElapsedMilliseconds} ms)");
                SafeWriteLine($"Reason: {ex.InnerException?.GetType().Name}: {ex.InnerException?.Message}");
                Interlocked.Increment(ref _failed);
            }
            catch (TimeoutException ex)
            {
                stopwatch.Stop();
                SafeWriteLine($"[FAILED] {entry.Method.Name} ({stopwatch.ElapsedMilliseconds} ms)");
                SafeWriteLine($"Reason: TimeoutException: {ex.Message}");
                Interlocked.Increment(ref _failed);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                SafeWriteLine($"[FAILED] {entry.Method.Name} ({stopwatch.ElapsedMilliseconds} ms)");
                SafeWriteLine($"Reason: {ex.GetType().Name}: {ex.Message}");
                Interlocked.Increment(ref _failed);
            }

            SafeWriteLine("");
        }

        private async Task ExecuteWithTimeout(TestExecutionEntry entry, int timeoutMs)
        {
            var task = Task.Run(() => entry.Method!.Invoke(entry.Instance, entry.Parameters));

            var completed = await Task.WhenAny(task, Task.Delay(timeoutMs));

            if (completed != task)
                throw new TimeoutException($"Test exceeded timeout of {timeoutMs} ms.");

            await task;
        }

        private void InvokeLifecycleMethod(MethodInfo method, object? instance, string stage)
        {
            try
            {
                method.Invoke(instance, null);
                SafeWriteLine($"[{stage}] {method.Name} completed successfully.");
            }
            catch (TargetInvocationException ex)
            {
                SafeWriteLine($"[{stage}] {method.Name} failed: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                SafeWriteLine($"[{stage}] {method.Name} failed: {ex.Message}");
            }
        }

        private void PrintResults(string title, TimeSpan elapsed)
        {
            SafeWriteLine("");
            SafeWriteLine($"===== {title} =====");
            SafeWriteLine($"Passed : {_passed}");
            SafeWriteLine($"Failed : {_failed}");
            SafeWriteLine($"Skipped: {_skipped}");
            SafeWriteLine($"Total  : {_passed + _failed + _skipped}");
            SafeWriteLine($"Time   : {elapsed.TotalMilliseconds:F0} ms");
        }

        private void ResetCounters()
        {
            _passed = 0;
            _failed = 0;
            _skipped = 0;
        }

        private void SafeWriteLine(string text)
        {
            lock (_consoleLock)
            {
                Console.WriteLine(text);
            }
        }

        private bool IsStaticClass(Type type)
        {
            return type.IsAbstract && type.IsSealed;
        }

        private class TestExecutionEntry
        {
            public MethodInfo? Method { get; set; }
            public MethodInfo? LifecycleMethod { get; set; }
            public object? Instance { get; set; }
            public object[]? Parameters { get; set; }
            public bool IsSkipped { get; set; }
            public bool IsAfterAll { get; set; }
            public int Priority { get; set; }
            public string Author { get; set; } = "";
            public string Category { get; set; } = "";
            public int? TimeoutMilliseconds { get; set; }
        }
    }
}
