using System;
using System.Collections.Generic;
using WIS.Domain.Documento.Integracion.Egreso;

namespace WIS.Domain.Documento
{
    public class DocumentoLineaEgreso
    {
        public string NumeroDocumento { get; set; }
        public string TpDocumento { get; set; }
        public int Numero { get; set; } //NU_SECUENCIA
        public int Empresa { get; set; }
        public string Producto { get; set; }
        public decimal Faixa { get; set; }
        public string Identificador { get; set; }
        public decimal? CantidadDesafectada { get; set; }
        public int Usuario { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public decimal? CantidadDescargada { get; set; }
        public decimal? FOB { get; set; }
        public decimal? CIF { get; set; }
        public decimal? Tributo { get; set; }
        public string NumeroProceso { get; set; }
        public string Semiacabado { get; set; }
        public string Consumible { get; set; }
        public string TpDocumentoIngreso { get; set; }
        public string NumeroDocumentoIngreso { get; set; }
        public IDocumento DocumentoIngreso { get; set; }
        public List<InformacionReserva> Reservas { get; set; }

        public DocumentoLineaEgreso()
        {
            Reservas = new List<InformacionReserva>();
        }
    }
}
