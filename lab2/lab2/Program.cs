using lab2.Project;
using lab2.Testing;
using lab2.Tests;
using System;
using System.Reflection;

namespace lab2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var assembly = typeof(Project.TempConver).Assembly;

            var runner = new Runner
            {
                MaxDegreeOfParallelism = 4
            };

            runner.RunAllTests(assembly);
        }
    
    }
}
