using WIS.Domain.Documento;
using WIS.Domain.Documento.TiposDocumento;

namespace WIS.Domain.DataModel
{
    public class FactoryService : IFactoryService
    {
        public FactoryService()
        {
        }

        public IDocumentoIngreso CreateDocumentoIngreso(string tipo)
        {
            return new DocumentoIngreso();
        }

        public IDocumentoEgreso CreateDocumentoEgreso(string tipo)
        {
            return new DocumentoEgreso();
        }

        public IDocumentoActa CreateDocumentoActa(string tipo)
        {
            return new DocumentoActa();
        }

        public IDocumentoAgrupador CreateDocumentoAgrupador(string tipoAgrupador)
        {
            return new DocumentoAgrupador();
        }
    }
}
