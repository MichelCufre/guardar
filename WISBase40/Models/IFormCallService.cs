using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WIS.FormComponent.Execution;
using WIS.FormComponent.Execution.Serialization;

namespace WIS.WebApplication.Models
{
    public interface IFormCallService
    {
        Task<IFormWrapper> CallFormServiceAsync(ServerRequest request, FormAction action, CancellationToken cancelToken);
    }
}
