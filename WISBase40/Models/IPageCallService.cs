using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WIS.PageComponent.Execution;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.WebApplication.Models
{
    public interface IPageCallService
    {
        Task<IPageWrapper> CallPageServiceAsync(ServerRequest request, PageAction action, CancellationToken cancelToken);
    }
}
