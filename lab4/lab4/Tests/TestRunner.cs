using lab4.Tests.Assertions;
using lab4.Tests.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace lab4.Tests.Core
{
    public class TestRunner
    {
        public async Task<List<TestResult>> RunAsync(Assembly assembly, TestFilter? filter = null)
        {
            var results = new List<TestResult>();
            var entries = TestDiscoverer.DiscoverTests(assembly);

            if (filter != null)
                entries = entries.Where(e => filter(e)).ToList();

            foreach (var entry in entries)
            {
                var result = new TestResult
                {
                    TestName = entry.DisplayName,
                    Passed = false,
                    Skipped = entry.IsSkipped
                };

                if (entry.IsSkipped)
                {
                    result.Message = "Skipped";
                    results.Add(result);
                    continue;
                }

                var beforeMethods = entry.Instance.GetType().GetMethods()
                    .Where(m => m.GetCustomAttribute<BeforeAttribute>() != null)
                    .ToList();

                var afterMethods = entry.Instance.GetType().GetMethods()
                    .Where(m => m.GetCustomAttribute<AfterAttribute>() != null)
                    .ToList();

                var sw = Stopwatch.StartNew();

                try
                {
                    foreach (var before in beforeMethods)
                        before.Invoke(entry.Instance, null);

                    object? invokeResult = entry.Method.Invoke(entry.Instance, entry.Parameters);
                    if (invokeResult is Task task)
                        await task;

                    result.Passed = true;
                    result.Message = "Passed";
                }
                catch (TargetInvocationException ex)
                {
                    result.Passed = false;
                    result.Message = ex.InnerException?.Message ?? ex.Message;
                }
                catch (AssertException ex)
                {
                    result.Passed = false;
                    result.Message = ex.Message;
                }
                catch (Exception ex)
                {
                    result.Passed = false;
                    result.Message = ex.Message;
                }
                finally
                {
                    foreach (var after in afterMethods)
                        after.Invoke(entry.Instance, null);

                    sw.Stop();
                    result.DurationMs = sw.ElapsedMilliseconds;
                }

                results.Add(result);
            }

            return results;
        }
    }
}
