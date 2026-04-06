using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace WIS.TaskQueue.Services
{
    public class TaskQueueHostedService : IHostedService
    {
        protected readonly ITaskQueue _taskQueue;

        public TaskQueueHostedService(ITaskQueue taskQueue)
        {
            _taskQueue = taskQueue;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _taskQueue.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _taskQueue.Stop();
            return Task.CompletedTask;
        }
    }
}
