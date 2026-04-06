using System;

namespace WIS.Domain.General
{
    public class CodigoMultidatoEmpresa
    {
        public int CodigoEmpresa { get; set; } //CD_EMPRESA
        public string CodigoMultidato { get; set; } //CD_CODIGO_MULTIDATO
        public string Habilitado { get; set; } //FL_HABILITADO
        public DateTime FechaAlta { get; set; } //DT_ADDROW
        public DateTime? FechaModificacion { get; set; } //DT_UPDROW
        public long? NumeroTransaccion { get; set; } //NU_TRANSACCION
        public long? NumeroTransaccionDelete { get; set; } //NU_TRANSACCION_DELETE
    }
}
