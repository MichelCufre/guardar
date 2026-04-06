using Dispatch;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Security;

namespace WIS.TaskQueue.Services
{
    public class TaskQueue : ITaskQueue, IHostedService, IDisposable
    {
        private Dictionary<string, List<Models.Task>> _tasks = new Dictionary<string, List<Models.Task>>();
        private Dictionary<string, List<string>> _precedingCategories = new Dictionary<string, List<string>>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private TaskProcessor _taskProcessor = null;
        private TaskLoader _taskLoader = null;
        private bool _enabled = true;
        private bool _preloadCompleted = false;
        private ITaskQueueService _taskQueueService;
        private ILogger<TaskQueue> _logger;
        private IOptions<TaskQueueSettings> _configuration;
        private IIdentityService _identity;
        private static readonly SerialQueue Queue = new SerialQueue();
        private static SerialQueue QueueInit = new SerialQueue();

        public TaskQueue(IOptions<TaskQueueSettings> configuration,
            ITaskQueueService taskQueueService,
            ILogger<TaskQueue> logger,
            IIdentityService identity)
        {
            _enabled = configuration.Value.IsEnabled;
            _taskQueueService = taskQueueService;
            _configuration = configuration;
            _logger = logger;
            _identity = identity;
        }

        public bool IsPreloadCompleted()
        {
            return _preloadCompleted;
        }

        public void Add(Models.Task task)
        {
            Queue.DispatchSync(() =>
            {
                if (_enabled)
                {
                    string key = (task.Category ?? "").ToLower();

                    _lock.EnterWriteLock();

                    if (!_tasks.ContainsKey(key))
                    {
                        _tasks[key] = new List<Models.Task>();
                    }

                    if (!_tasks[key].Exists(t => Models.Task.AreEqual(task, t)))
                    {
                        _tasks[key].Add(task);
                    }

                    _lock.ExitWriteLock();
                }
            });
        }

        public void Remove(Models.Task task)
        {
            Queue.DispatchSync(() =>
            {
                string key = (task.Category ?? "").ToLower();

                _lock.EnterWriteLock();

                if (_tasks.ContainsKey(key))
                {
                    _tasks[key].Remove(task);
                }

                _lock.ExitWriteLock();
            });
        }

        public IEnumerable<Models.Task> GetTasks(string category, int count)
        {
            string key = (category ?? "").ToLower();
            List<Models.Task> tasks = new List<Models.Task>();

            Queue.DispatchSync(() =>
            {
                _lock.EnterReadLock();

                if (_tasks.ContainsKey(key))
                {
                    tasks.AddRange(_tasks[key].Take(count));
                }

                _lock.ExitReadLock();
            });

            return tasks;
        }

        public IEnumerable<string> GetCategories()
        {
            List<string> categories = new List<string>();

            _lock.EnterReadLock();

            categories.AddRange(_tasks.Keys);

            _lock.ExitReadLock();

            return categories;
        }

        public void Start()
        {
            SetIdentity();

            if (_enabled)
            {
                Thread tr = new Thread(new ThreadStart(Init));
                tr.IsBackground = true;
                tr.Start();
            }
        }

        private void SetIdentity()
        {
            var manager = (IIdentityServiceManager)_identity;
            manager.SetUser(new BasicUserData
            {
                Language = "es",
                UserId = -1
            }, "TaskQueue", GeneralDb.PredioSinDefinir);
        }

        private void Init()
        {
            QueueInit.DispatchSync(() =>
            {
                _preloadCompleted = false;
                _precedingCategories.Clear();
                _tasks.Clear();
                _taskLoader = new TaskLoader(this, _taskQueueService, _configuration);
                _taskLoader.LoadTasks();
                _preloadCompleted = true;
                _taskProcessor = new TaskProcessor(this, _taskQueueService, _configuration, _logger);
                _taskProcessor.Start().Wait();
            });
        }

        ~TaskQueue()
        {
            Dispose();
        }

        public void Stop()
        {
            if (_taskProcessor != null)
            {
                _taskProcessor.Stop();
            }

            if (QueueInit != null)
                QueueInit = new SerialQueue();
        }

        public void AddCategoryPrecedence(string before, string after)
        {
            string key = (after ?? "").ToLower();
            before = (before ?? "").ToLower();

            _lock.EnterReadLock();

            if (!_precedingCategories.ContainsKey(key))
            {
                _precedingCategories[key] = new List<string>();
            }

            if (!_precedingCategories[key].Contains(before))
            {
                _precedingCategories[key].Add(before);
            }

            _lock.ExitReadLock();
        }

        public bool HasPrecedingTasks(string category)
        {
            string key = (category ?? "").ToLower();

            _lock.EnterReadLock();

            var response = _precedingCategories.ContainsKey(key) && _precedingCategories[key].Exists(c => HasTasks(c));

            _lock.ExitReadLock();

            return response;
        }

        private bool HasTasks(string category)
        {
            string key = (category ?? "").ToLower();
            return _tasks.ContainsKey(key) && _tasks[key].Count > 0;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Stop();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_lock != null)
            {
                _lock.Dispose();
            }

            if (_taskProcessor != null)
            {
                _taskProcessor.Stop();
            }
        }
    }
}
