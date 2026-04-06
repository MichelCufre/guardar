using System.Collections.Generic;
using System.Threading.Tasks;

namespace WIS.Domain.Services.Interfaces
{
    public interface ITaskQueueService
    {
        List<Dictionary<string, string>> LoadTasks(string category);
        Task Init();
        Task Process(string category, Dictionary<string, string> data);
        bool IsEnabled();
        bool IsOnDemandReportProcessing();
        void Enqueue(string category, List<string> keys);
        void Enqueue(string category, int interfazExterna, string key);
        void Enqueue(string category, int interfazExterna, List<string> keys);
        void Restart();
    }
}
