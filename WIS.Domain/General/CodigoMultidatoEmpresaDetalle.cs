using System;

namespace WIS.Domain.General
{
    public class CodigoMultidatoEmpresaDetalle
    {
        public int CodigoEmpresa { get; set; } //CD_EMPRESA
        public string CodigoMultidato { get; set; } //CD_CODIGO_MULTIDATO
        public string CodigoAplicacion { get; set; } //CD_APLICACION
        public string CodigoCampo { get; set; } //CD_CAMPO
        public string CodigoAI { get; set; } //CD_AI
        public DateTime FechaAlta { get; set; } //DT_ADDROW
        public DateTime? FechaModificacion { get; set; } //DT_UPDROW
        public long? NumeroTransaccion { get; set; } //NU_TRANSACCION
        public long? NumeroTransaccionDelete { get; set; } //NU_TRANSACCION_DELETE
        public short NumeroOrden { get; set; } //NU_ORDEN
    }
}
