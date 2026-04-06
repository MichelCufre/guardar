using System;

namespace WIS.Domain.Documento
{
    public class DocumentoProduccion
    {
        public string NumeroProduccion { get; set; }
        public DateTime? FechaAlta { get; set; }                //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }        //DT_UPDROW
        public IDocumentoIngreso DocumentoIngreso { get; set; }
        public IDocumentoEgreso DocumentoEgreso { get; set; }
    }
}
