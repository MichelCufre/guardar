using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WIS.WebApplication.Models
{
    public interface IPageService
    {
        Task<ServerResponse> Load(ServerRequest serverRequest, CancellationToken cancelToken);
        Task<ServerResponse> Unload(ServerRequest serverRequest, CancellationToken cancelToken);
    }
}
