using System.Collections.Generic;
using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.TaskQueue.Dtos
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
        /// <summary>
        /// Categoria de la tarea
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RequiredValidation]
        public string Category { get; set; }

        /// <summary>
        /// Datos de la tarea
        /// </summary>
        /// <example></example>
        [RequiredValidation]
        public Dictionary<string, string> Data { get; set; }
    }
}
