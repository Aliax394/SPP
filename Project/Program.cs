namespace Project
{
    public class MyProject
    {
        private readonly List<string> _tasks = new();
        private readonly HashSet<int> _completed = new();

        public int Count => _tasks.Count;

        public void Add(string task)
        {
            if (string.IsNullOrWhiteSpace(task))
                throw new ArgumentException("Task cannot be empty");

            _tasks.Add(task);
        }

        public void Remove(int index)
        {
            if (index < 0 || index >= _tasks.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            _tasks.RemoveAt(index);
            _completed.Remove(index);
        }

        public void MarkCompleted(int index)
        {
            if (index < 0 || index >= _tasks.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            _completed.Add(index);
        }

        public bool IsCompleted(int index)
        {
            if (index < 0 || index >= _tasks.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return _completed.Contains(index);
        }

        public string Get(int index)
        {
            if (index < 0 || index >= _tasks.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return _tasks[index];
        }
    }
}