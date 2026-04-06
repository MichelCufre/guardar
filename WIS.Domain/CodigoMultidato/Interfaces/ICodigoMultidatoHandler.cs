using System.Collections.Generic;
using WIS.Domain.DataModel;

namespace WIS.Domain.CodigoMultidato.Interfaces
{
    public interface ICodigoMultidatoHandler
    {
        string GetCodigo();
        Dictionary<string, string> GetAIs(IUnitOfWork uow, string codigo);
        bool IsValid(IUnitOfWork uow, string codigo, out Dictionary<string, string> ais);
        int? GetEmpresa(IUnitOfWork uow, int userId, Dictionary<string, string> ais, CodigoMultidatoTipoLectura tipoLectura, int? empresa);
        object GetAIValue(Dictionary<string, string> ais, Dictionary<string, string> aiTypes, Dictionary<string, decimal> aiConversions, string fnc1, string ai);
    }
}
