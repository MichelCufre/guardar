using System.Threading.Tasks;
using WIS.Common.API.Dtos;

namespace WIS.MultidataCodeConverter.Interfaces
{
    public interface IMultidataCodeConverterService
    {
        Task<MultidataCodeConversionResponse> Convert(MultidataCodeConversionRequest request);
    }
}
