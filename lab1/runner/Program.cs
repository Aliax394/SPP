using System.Reflection;
using Library;

namespace Runner
{
    public class Program
    {
        public static async Task Main()
        {
            var assembly = typeof(Test.Testing).Assembly;

            int pass = 0;
            int fail = 0;

            var testCl = assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<TestClassAttribute>() != null);

            foreach (var type in testCl)
            {
                Console.WriteLine($"\n=== {type.Name} ===");

                var instance = Activator.CreateInstance(type);
                

                var before = type.GetMethods()
                    .FirstOrDefault(m => m.GetCustomAttribute<BeforeAttribute>() != null);

                var after = type.GetMethods()
                    .FirstOrDefault(m => m.GetCustomAttribute<AfterAttribute>() != null);

                before?.Invoke(instance, null);

                var tests = type.GetMethods()
                    .Select(m => new
                    {
                        Method = m,
                        Attr = m.GetCustomAttribute<TestAttribute>()
                    })
                    .Where(x => x.Attr != null && x.Attr.Enabled)
                    .OrderByDescending(x => x.Attr!.Priority)
                    .ToList();

                foreach (var test in tests)
                {
                    var method = test.Method;

                    var cases = method.GetCustomAttributes<TestCaseAttribute>().ToArray();

                    if (cases.Length == 0)
                    {
                        await RunTest(instance, method, null);
                    }
                    else
                    {
                        foreach (var c in cases)
                            await RunTest(instance, method, c.Parameters);
                    }
                }

                after?.Invoke(instance, null);
                
            }

            Console.WriteLine($"\nRESULT: Passed = {pass}, Failed = {fail}");

            async Task RunTest(object? instance, MethodInfo method, object?[]? parameters)
            {
                string name = method.Name;

                if (parameters != null)
                {
                    var paramStrings = parameters.Select(p =>
                    {
                        if (p == null)
                            return "null";

                        if (p is string str)
                            return $"\"{str}\"";

                        if (p is System.Collections.IEnumerable enumerable)
                        {
                                var items = enumerable.Cast<object>()
                                                      .Select(x => x?.ToString() ?? "null");
                                return $"[{string.Join(", ", items)}]";
                        }

                        return p.ToString();
                    });
                    name += $"({string.Join(", ", paramStrings)})";
                }

                try
                {
                    var result = method.Invoke(instance, parameters);

                    if (result is Task task)
                        await task;

                    Console.WriteLine($"+ {name}");
                    pass++;
                }
                catch (TargetInvocationException ex)
                {
                    Console.WriteLine($"- {name}");
                    Console.WriteLine($"   {ex.InnerException?.Message}");
                    fail++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"- {name}");
                    Console.WriteLine($"   {ex.Message}");
                    fail++;
                }
            }
        }
    }
}
