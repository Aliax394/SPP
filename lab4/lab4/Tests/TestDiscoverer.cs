using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace lab4.Tests.Core
{
    public static class TestDiscoverer
    {
        public static List<TestExecutionEntry> DiscoverTests(Assembly assembly)
        {
            var entries = new List<TestExecutionEntry>();

            var testClasses = assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<Tests.TestClassAttr>() != null);

            foreach (var type in testClasses)
            {
                object instance = Activator.CreateInstance(type)!;

                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                foreach (var method in methods)
                {
                    var testAttr = method.GetCustomAttribute<TestAttribute>();
                    if (testAttr == null)
                        continue;

                    var authorAttr = method.GetCustomAttribute<AuthorAttribute>();
                    var categoryAttr = method.GetCustomAttribute<CategoryAttribute>();
                    var sourceAttr = method.GetCustomAttribute<TestCaseSourceAttribute>();

                    if (!testAttr.Enabled)
                    {
                        entries.Add(new TestExecutionEntry
                        {
                            Method = method,
                            Instance = instance,
                            Parameters = null,
                            IsSkipped = true,
                            Priority = testAttr.Priority,
                            Author = authorAttr?.Name ?? "Unknown",
                            Category = categoryAttr?.Name ?? "Unspecified",
                            DisplayName = method.Name
                        });
                        continue;
                    }

                    if (sourceAttr == null)
                    {
                        entries.Add(new TestExecutionEntry
                        {
                            Method = method,
                            Instance = instance,
                            Parameters = null,
                            IsSkipped = false,
                            Priority = testAttr.Priority,
                            Author = authorAttr?.Name ?? "Unknown",
                            Category = categoryAttr?.Name ?? "Unspecified",
                            DisplayName = method.Name
                        });
                    }
                    else
                    {
                        var generatedCases = GetCases(type, instance, sourceAttr.SourceName);
                        foreach (var testCase in generatedCases)
                        {
                            entries.Add(new TestExecutionEntry
                            {
                                Method = method,
                                Instance = instance,
                                Parameters = testCase.Arguments,
                                IsSkipped = false,
                                Priority = testAttr.Priority,
                                Author = authorAttr?.Name ?? "Unknown",
                                Category = categoryAttr?.Name ?? "Unspecified",
                                DisplayName = testCase.Name ?? method.Name
                            });
                        }
                    }
                }
            }

            return entries
                .OrderByDescending(e => e.Priority)
                .ThenBy(e => e.DisplayName)
                .ToList();
        }
        private static IEnumerable<TestCaseData> GetCases(Type type, object instance, string sourceName)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

            var member = type.GetMember(sourceName, flags).FirstOrDefault();
            if (member == null)
                throw new Exception($"Source '{sourceName}' not found in {type.Name}");

            object? value = member switch
            {
                MethodInfo method => method.Invoke(method.IsStatic ? null : instance, null),
                PropertyInfo property => property.GetValue(property.GetMethod!.IsStatic ? null : instance),
                FieldInfo field => field.GetValue(field.IsStatic ? null : instance),
                _ => null
            };

            if (value is IEnumerable<TestCaseData> typedCases)
                return typedCases;

            if (value is IEnumerable raw)
            {
                var result = new List<TestCaseData>();
                foreach (var item in raw)
                {
                    if (item is TestCaseData tcd)
                        result.Add(tcd);
                }
                return result;
            }

            throw new Exception($"Source '{sourceName}' must return IEnumerable<TestCaseData>");
        }
    }
}
