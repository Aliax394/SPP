using System;
using System.Collections.Generic;
using System.Text;

namespace lab3.DynamicThPoolL
{
    public class WorkItem
    {
        public Action Action { get; }
        public DateTime EnqueueTime { get; }

        public WorkItem(Action action)
        {
            Action = action;
            EnqueueTime = DateTime.UtcNow;
        }
    }
}
