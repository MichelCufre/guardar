using System;

namespace WIS.Domain.ManejoStock
{
    public class DocumentoAjusteStock
    {
        public int NumeroAjuste { get; set; }
        public string Producto { get; set; }
        public decimal? Faixa { get; set; }
        public string Identificador { get; set; }
        public int? CodigoEmpresa { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public decimal? CantidadMovimiento { get; set; }
        public string DescripcionMotivo { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string CodigoMotivoAjuste { get; set; }
        public DateTime? FechaMotivo { get; set; }
        public int? FuncionarioMotivo { get; set; }
        public long? NumeroTransaccion { get; set; }
        public string Predio { get; set; }
        public string Aplicacion { get; set; }
        public int? CodigoFuncionario { get; set; }
        public string Endereco { get; set; }
    }
}
