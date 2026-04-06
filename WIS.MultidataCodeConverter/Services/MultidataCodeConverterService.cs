using System.Linq;
using System.Threading.Tasks;
using WIS.Common.API.Dtos;
using WIS.MultidataCodeConverter.Interfaces;

namespace WIS.MultidataCodeConverter.Services
{
    public class MultidataCodeConverterService : IMultidataCodeConverterService
    {
        public async Task<MultidataCodeConversionResponse> Convert(MultidataCodeConversionRequest request)
        {
            return new MultidataCodeConversionResponse
            {
                AIs = request.AIs.Select(x => new MultidataCodeConversionAIResponse
                {
                    AI = x.AI,
                    Value = x.Value,
                }).ToList(),
            };
        }
    }
}
