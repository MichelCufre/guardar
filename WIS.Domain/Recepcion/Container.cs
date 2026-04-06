using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Recepcion
{
    public class Container
    {
        public string Numero { get; set; }
        public short Id { get; set; }
        public string TP_CONTAINER { get; set; }
        public int? Empresa { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public DateTime? DT_VEN_RET_TERMINAL { get; set; }
        public DateTime? DT_FIN_RET_TERMINAL { get; set; }
        public DateTime? DT_MAX_DEVOLUCION { get; set; }
        public DateTime? DT_INI_DEVOLUCION { get; set; }
        public DateTime? DT_FIN_DEVOLUCION { get; set; }
        public DateTime? DT_FIN_APERTURA { get; set; }
        public short? CD_SITUACAO { get; set; }
        public decimal? PS_TARA { get; set; }
        public string ID_CONSOLIDADO { get; set; }
        public short? CD_ENDERECO_DEPOSITO { get; set; }
        public int? CD_FUNC_ENCARGADO { get; set; }
        public int? CD_FUNC_VERIFICADOR { get; set; }
        public string DS_MEMO { get; set; }
        public DateTime? DT_ENTREGA_DOCUMENTO { get; set; }
        public int? CD_TERMINAL_ENTREGA { get; set; }
        public int? CD_TERMINAL_DEVOLUCION { get; set; }
        public int? CD_TRANSPORTISTA_RETIRO { get; set; }
        public int? CD_TRANSPORTISTA_DEVOLUCION { get; set; }
        public string VL_PRECINTO_1 { get; set; }
        public string VL_PRECINTO_2 { get; set; }
        public string DS_OBSERVACIONES_1 { get; set; }
        public string DS_OBSERVACIONES_2 { get; set; }
        public string DS_BOOKING { get; set; }
        public DateTime? DT_POSICIONAMIENTO { get; set; }
        public DateTime? DT_APERTURA { get; set; }
        public string FL_PALETIZADO { get; set; }
        public decimal? CD_FUNCIONARIO_APERTURA { get; set; }
        public decimal? CD_FUNCIONARIO_CIERRE { get; set; }
        public DateTime? FechaEntrada { get; set; }
        public DateTime? FechaSalida { get; set; }
        public long? NumeroTransaccion { get; set; }
        public long? NumeroTransaccionDelete { get; set; }
    }
}
