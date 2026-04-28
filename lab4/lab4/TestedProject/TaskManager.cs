using System;
using System.Collections.Generic;
using System.Text;

namespace lab4.TestedProject
{
    public class TaskManager
    {
        private readonly List<string> _tasks = new();

        public int Count => _tasks.Count;

        public void Add(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Task text cannot be empty");

            _tasks.Add(text);
        }

        public string GetAt(int index)
        {
            return _tasks[index];
        }
    }
}
