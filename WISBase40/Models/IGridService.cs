using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WIS.WebApplication.Models
{
    public interface IGridService
    {
        Task<ServerResponse> Initialize(ServerRequest serverRequest, CancellationToken cancelToken);
        Task<ServerResponse> FetchRows(ServerRequest serverRequest, CancellationToken cancelToken);
        Task<ServerResponse> FetchStats(ServerRequest serverRequest, CancellationToken cancelToken);
        Task<ServerResponse> ValidateRow(ServerRequest serverRequest, CancellationToken cancelToken);
        Task<ServerResponse> Commit(ServerRequest serverRequest, CancellationToken cancelToken);
        Task<ServerResponse> ButtonAction(ServerRequest serverRequest, CancellationToken cancelToken);
        Task<ServerResponse> MenuItemAction(ServerRequest serverRequest, CancellationToken cancelToken);
        Task<ServerResponse> UpdateConfig(ServerRequest serverRequest, CancellationToken cancelToken);
        Task<ServerResponse> ExportExcel(ServerRequest serverRequest, CancellationToken cancelToken);
        Task<ServerResponse> ImportExcel(ServerRequest serverRequest, CancellationToken cancelToken);
        Task<ServerResponse> GenerateExcelTemplate(ServerRequest serverRequest, CancellationToken cancelToken);
        Task<ServerResponse> SelectSearch(ServerRequest serverRequest, CancellationToken cancelToken);
        Task<ServerResponse> SaveFilter(ServerRequest serverRequest, CancellationToken cancelToken);
        Task<ServerResponse> RemoveFilter(ServerRequest serverRequest, CancellationToken cancelToken);
        Task<ServerResponse> GetFilterList(ServerRequest serverRequest, CancellationToken cancelToken);
        Task<ServerResponse> NotifySelection(ServerRequest serverRequest, CancellationToken cancelToken);
        Task<ServerResponse> NotifyInvertSelection(ServerRequest serverRequest, CancellationToken cancelToken);
    }
}
