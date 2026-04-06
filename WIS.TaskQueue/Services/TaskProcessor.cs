using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;

namespace WIS.TaskQueue.Services
{
    public class TaskProcessor : IDisposable
    {
        private bool _stop = false;
        private ITaskQueue _taskQueue;
        private ITaskQueueService _taskQueueService;
        private ILogger<TaskQueue> _logger;
        private int _noTasksSleep;
        private int _iterastionSleep;
        private int _maxTaskProcessingPerCategory;

        public TaskProcessor(
            ITaskQueue taskQueue, 
            ITaskQueueService taskQueueService, 
            IOptions<TaskQueueSettings> configuration, 
            ILogger<TaskQueue> logger)
        {
            _taskQueue = taskQueue;
            _taskQueueService = taskQueueService;
            _logger = logger;
            _noTasksSleep = configuration.Value.NoTasksSleepInMilliseconds;
            _iterastionSleep = configuration.Value.IterationSleepInMilliseconds;
            _maxTaskProcessingPerCategory = configuration.Value.MaxTaskProcessingPerCategory;
        }

        public async Task Start()
        {
            _stop = false;

            await _taskQueueService.Init();

            while (!_stop)
            {
                await ProcessTasks();
            }
        }

        private async Task ProcessTasks()
        {
            bool noTasks = true;

            foreach (var category in _taskQueue.GetCategories())
            {
                if (_taskQueue.HasPrecedingTasks(category))
                {
                    continue;
                }

                foreach (var task in _taskQueue.GetTasks(category, _maxTaskProcessingPerCategory))
                {
                    if (_stop)
                    {
                        return;
                    }

                    noTasks = false;
                    await Process(task);
                    _taskQueue.Remove(task);
                }
            }

            Thread.Sleep(noTasks ? _noTasksSleep : _iterastionSleep);
        }

        private async Task Process(Models.Task task)
        {
            try
            {
                await _taskQueueService.Process(task.Category, task.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public void Stop()
        {
            _stop = true;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
