using System.Threading;
using System.Threading.Tasks;

namespace WIS.WebApplication.Models
{
    public interface ITrafficOfficerFrontendService
    {
        string SessionToken { get; }
        bool TooManySessions { get; }
        Task<bool> ClearToken(string token, string application, CancellationToken cancelToken);
        Task<bool> DeleteUserLocks(CancellationToken cancelToken);
        Task CreateSession(CancellationToken cancelToken);
        Task<bool> IsSessionValid(CancellationToken cancelToken);
        Task UpdateSessionActivity(CancellationToken cancelToken);
        Task RemoveSession(CancellationToken cancelToken);
        Task UpdateThreadOperationActivity(string pageToken, CancellationToken cancelToken);
    }
}
