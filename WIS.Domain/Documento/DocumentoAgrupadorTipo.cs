using System.Collections.Generic;

namespace WIS.Domain.Documento
{
    public class DocumentoAgrupadorTipo
    {
        public string TipoAgrupador { get; set; }
        public string Descripcion { get; set; }
        public bool Habilitado { get; set; }
        public string TipoOperacion { get; set; }
        public int? CantidadMaximaDocumentos { get; set; }
        public bool ManejaPredio { get; set; }
        public string Secuencia { get; set; }
        public List<DocumentoAgrupadorGrupo> Grupos { get; set; }
    }
}
