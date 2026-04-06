using System;

namespace WIS.Domain.ManejoStock
{
    public class DocumentoAjusteStockHistorico
    {
        public DocumentoAjusteStockHistorico(){ }

        public DocumentoAjusteStockHistorico(DocumentoAjusteStock ajusteOriginal)
        {
            this.Aplicacion = ajusteOriginal.Aplicacion;
            this.CodigoEmpresa = ajusteOriginal.CodigoEmpresa;
            this.CodigoFuncionario = ajusteOriginal.CodigoFuncionario;
            this.CodigoMotivoAjuste = ajusteOriginal.CodigoMotivoAjuste;
            this.DescripcionMotivo = ajusteOriginal.DescripcionMotivo;
            this.Endereco = ajusteOriginal.Endereco;
            this.Faixa = ajusteOriginal.Faixa;
            this.FechaActualizacion = DateTime.Now;
            this.FechaCreacion = DateTime.Now;
            this.FechaMotivo = ajusteOriginal.FechaMotivo;
            this.FuncionarioMotivo = ajusteOriginal.FuncionarioMotivo;
            this.Identificador = ajusteOriginal.Identificador;
            this.NumeroAjuste = ajusteOriginal.NumeroAjuste;
            this.NumeroDocumento = ajusteOriginal.NumeroDocumento;
            this.NumeroTransaccion = ajusteOriginal.NumeroTransaccion;
            this.Producto = ajusteOriginal.Producto;
            this.Predio = ajusteOriginal.Predio;
            this.TipoDocumento = ajusteOriginal.TipoDocumento;
        }

        public int NumeroAjuste { get; set; }
        public string TipoOperacion { get; set; }
        public int NumeroOperacion { get; set; }
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
