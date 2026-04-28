using System;
using System.Collections.Generic;
using System.Text;

namespace lab4.Tests.Filters
{
    public static class FilterFactory
    {
        public static TestFilter ByCategory(string category)
            => entry => entry.Category.Equals(category, System.StringComparison.OrdinalIgnoreCase);

        public static TestFilter ByAuthor(string author)
            => entry => entry.Author.Equals(author, System.StringComparison.OrdinalIgnoreCase);

        public static TestFilter ByMinPriority(int minPriority)
            => entry => entry.Priority >= minPriority;

        public static TestFilter CombineAnd(params TestFilter[] filters)
            => entry => filters.All(f => f(entry));
    }
}
