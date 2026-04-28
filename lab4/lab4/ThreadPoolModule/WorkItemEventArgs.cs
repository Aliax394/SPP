using System;
using System.Collections.Generic;
using System.Text;

namespace lab4.ThreadPoolModule
{
    public class WorkItemEventArgs : EventArgs
    {
        public string Message { get; }
        public int WorkerId { get; }

        public WorkItemEventArgs(string message, int workerId)
        {
            Message = message;
            WorkerId = workerId;
        }
    }
}
