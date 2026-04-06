using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WIS.WebApplication.Models
{
    public interface IFormService
    {
        Task<ServerResponse> Initialize(ServerRequest serverRequest, CancellationToken cancelToken);
        Task<ServerResponse> ValidateField(ServerRequest serverRequest, CancellationToken cancelToken);
        Task<ServerResponse> Submit(ServerRequest serverRequest, CancellationToken cancelToken);
        Task<ServerResponse> ButtonAction(ServerRequest serverRequest, CancellationToken cancelToken);
        Task<ServerResponse> SelectSearch(ServerRequest serverRequest, CancellationToken cancelToken);
    }
}
