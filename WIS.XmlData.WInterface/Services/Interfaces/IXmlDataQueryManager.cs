using WIS.Persistence.Database;

namespace WIS.XmlData.WInterface.Services.Interfaces
{
    public interface IXmlDataQueryManager
    {
        public Task<string> GetXmlData(WISDB context, string token, string loginName, int interfazExterna, int empresa, long nuEjecucion);
    }
}
