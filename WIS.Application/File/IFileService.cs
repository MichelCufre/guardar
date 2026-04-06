using System.Threading.Tasks;
using WIS.File.Execution;

namespace WIS.Application.File
{
    public interface IFileService
    {
        Task<FileDownloadResponse> GetFile(int user, FileDownloadRequest request);
        FileUploadResponse AddFile(int user, FileUploadRequest request);
        FileDeleteResponse DeleteFile(int user, FileDeleteRequest request);
    }
}
