using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Services.Configuracion
{
    public class TaskQueueSettings
    {
        //Propipedades de uso comun
        public const string Position = "TaskQueueSettings";
        public bool IsEnabled { get; set; }

        //Propiedades para proyectos que hacen uso de WIS.Taskqueue (WIS.WMS_API, WIS.BackendService, ...etc.)
        public string Endpoint { get; set; }

        //Propiedades para WIS.Taskqueue
        public int NoTasksSleepInMilliseconds { get; set; }
        public int IterationSleepInMilliseconds { get; set; }
        public int MaxTaskProcessingPerCategory { get; set; }
        public string WebHookNotificactionLogPath { get; set; }
        public List<CategoryPrecedence> CategoryPrecedences { get; set; }
    }

    public class CategoryPrecedence
    {
        public string Before { get; set; }
        public string After { get; set; }
    }
}
