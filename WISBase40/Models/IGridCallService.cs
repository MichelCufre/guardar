using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WIS.GridComponent.Execution;
using WIS.GridComponent.Execution.Serialization;

namespace WIS.WebApplication.Models
{
    public interface IGridCallService
    {
        Task<IGridWrapper> CallGridServiceAsync(ServerRequest request, GridAction action, CancellationToken cancelToken);
        Task<IGridWrapper> CallGridServiceAsync(ServerRequest request, string url, GridAction action, CancellationToken cancelToken);
    }
}
