using WIS.Domain.Documento;

namespace WIS.Domain.DataModel
{
    public interface IFactoryService
    {
        IDocumentoIngreso CreateDocumentoIngreso(string tipo);
        IDocumentoEgreso CreateDocumentoEgreso(string tipo);
        IDocumentoActa CreateDocumentoActa(string tipo);
        IDocumentoAgrupador CreateDocumentoAgrupador(string tipoAgrupador);
    }
}
