using System;
using System.Collections.Generic;
using WIS.TaskQueue.Models;

namespace WIS.TaskQueue.Services
{
    public interface ITaskQueue
    {
        bool IsPreloadCompleted();

        void Start();

        void Add(Task task);

        IEnumerable<string> GetCategories();

        IEnumerable<Task> GetTasks(string category, int count);

        void Remove(Task task);

        void Stop();

        void AddCategoryPrecedence(string before, string after);

        bool HasPrecedingTasks(string category);
    }
}
