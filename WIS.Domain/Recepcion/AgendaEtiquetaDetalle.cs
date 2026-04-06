using System;

namespace WIS.Domain.Recepcion
{
    public class AgendaEtiquetaDetalle
    {
        public string NumeroEtiquetaLote { get; set; }          // NU_ETIQUETA_LOTE
        public string CodigoProducto { get; set; }              // CD_PRODUTO
        public int? CodigoFaixa { get; set; }                   // CD_FAIXA
        public decimal? CodigoEmpresa { get; set; }             // CD_EMPRESA
        public string NumeroIdentificador { get; set; }         // NU_IDENTIFICADOR
        public decimal? CantidadProductoRecibido { get; set; }  // QT_PRODUTO_RECIBIDO
        public decimal? CantidadProducto { get; set; }          // QT_PRODUTO
        public decimal? CantidadAjusteRecibido { get; set; }    // QT_AJUSTE_RECIBIDO
        public decimal? CantidadEtiquetaGenerada { get; set; }  // QT_ETIQUETA_GENERADA
        public decimal? CantidadAlmacenado { get; set; }        // QT_ALMACENADO
        public DateTime? FechaFabricacion { get; set; }         // DT_FABRICACAO
    }
}
