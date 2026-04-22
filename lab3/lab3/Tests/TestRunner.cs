using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace lab3.Tests
{
    public static class TestRunner
    {
        public static List<Action> BuildTestsFromAssembly(Assembly assembly, Action<TestResult> onResult)
        {
            var jobs = new List<Action>();

            var testClasses = assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<TestClassAttr>() != null);

            foreach (var type in testClasses)
            {
                var methods = type.GetMethods()
                    .Where(m => m.GetCustomAttribute<TestAttr>() != null)
                    .OrderByDescending(m => m.GetCustomAttribute<TestAttr>()!.Priority);

                foreach (var method in methods)
                {
                    var testAttr = method.GetCustomAttribute<TestAttr>()!;
                    if (!testAttr.Enabled)
                        continue;

                    var testCases = method.GetCustomAttributes<TestCaseAttr>().ToArray();

                    if (testCases.Length == 0)
                    {
                        jobs.Add(() => RunSingleTest(type, method, null, onResult));
                    }
                    else
                    {
                        foreach (var testCase in testCases)
                        {
                            jobs.Add(() => RunSingleTest(type, method, testCase.Parameters, onResult));
                        }
                    }
                }
            }

            return jobs;
        }

        private static void RunSingleTest(Type type, MethodInfo method, object?[]? parameters, Action<TestResult> onResult)
        {
            var result = new TestResult
            {
                className = type.Name,
                methodName = method.Name
            };

            var sw = Stopwatch.StartNew();

            try
            {
                var instance = Activator.CreateInstance(type)
                    ?? throw new Exception($"Cannot create instance of {type.Name}");

                var before = type.GetMethods().FirstOrDefault(m => m.GetCustomAttribute<BeforeAttr>() != null);
                var after = type.GetMethods().FirstOrDefault(m => m.GetCustomAttribute<AfterAttr>() != null);

                before?.Invoke(instance, null);
                method.Invoke(instance, parameters);
                after?.Invoke(instance, null);

                result.passed = true;
                result.message = parameters == null
                    ? "OK"
                    : $"OK ({string.Join(", ", parameters.Select(p => p?.ToString() ?? "null"))})";
            }
            catch (TargetInvocationException ex)
            {
                result.passed = false;
                result.message = ex.InnerException?.Message ?? ex.Message;
            }
            catch (Exception ex)
            {
                result.passed = false;
                result.message = ex.Message;
            }
            finally
            {
                sw.Stop();
                result.durationMs = sw.ElapsedMilliseconds;
                onResult(result);
            }
        }
    }
}
