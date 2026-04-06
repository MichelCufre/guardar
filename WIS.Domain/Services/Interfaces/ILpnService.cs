using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services.Interfaces
{
    public interface ILpnService
    {
        Task<Lpn> GetLpn(long nuLpn);
        Task<ValidationsResult> AgregarLpns(List<Lpn> lpns, int empresa, int userId);
        Task<Lpn> GetLpn(string idExterno, string tpLpn);
    }
}
