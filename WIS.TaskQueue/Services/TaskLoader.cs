using Microsoft.Extensions.Options;
using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;

namespace WIS.TaskQueue.Services
{
    public class TaskLoader
    {
        private ITaskQueue _taskQueue;
        private ITaskQueueService _taskQueueService;
        private IOptions<TaskQueueSettings> _configuration;

        public TaskLoader(
            ITaskQueue taskQueue, 
            ITaskQueueService taskQueueService, 
            IOptions<TaskQueueSettings> configuration)
        {
            _taskQueue = taskQueue;
            _taskQueueService = taskQueueService;
            _configuration = configuration;
        }

        public void LoadTasks()
        {
            var categories = new List<string> 
            {
                TaskQueueCategory.WEBHOOK,
                TaskQueueCategory.API
            };

            if (_taskQueueService.IsOnDemandReportProcessing())
            {
                categories.Add(TaskQueueCategory.REPORT);
            }

            SetCategoryPrecedences();

            foreach (var category in categories)
            { 
                LoadTasks(category);
            }
        }

        private void LoadTasks(string category)
        {
            var tasks = _taskQueueService.LoadTasks(category);

            foreach (var task in tasks)
            {
                _taskQueue.Add(new Models.Task()
                {
                    Category = category,
                    Data = task
                });
            }
        }

        private void SetCategoryPrecedences()
        {
            foreach (var precedence in _configuration.Value.CategoryPrecedences)
            {
                _taskQueue.AddCategoryPrecedence(precedence.Before, precedence.After);
            }
        }
    }
}
