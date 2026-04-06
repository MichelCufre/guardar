using System;
using System.Collections.Generic;

namespace WIS.TaskQueue.Models
{
    public class Task
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Category { get; set; }

        public Dictionary<string, string> Data { get; set; }

        public override bool Equals(object? obj)
        {
            return Id == (obj as Task).Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Id + ":" + CreatedAt + ":" + Category + ":" + Data;
        }

        public static bool AreEqual(Task task, Task other)
        {
            return GetDataValue(task, "Interfaz") == GetDataValue(other, "Interfaz") &&
                GetDataValue(task, "Key") == GetDataValue(other, "Key");
        }

        public static string GetDataValue(Task task, string key)
        {
            return task.Data.ContainsKey(key) ? task.Data[key] : string.Empty;
        }
    }
}
