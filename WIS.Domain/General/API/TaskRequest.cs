using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.General.API
{
    public class TasksRequest
    {
        public List<TaskRequest> Tasks { get; set; }

        public TasksRequest()
        {
            Tasks = new List<TaskRequest>();
        }
    }
    public class TaskRequest
    {
        public string Category { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}
