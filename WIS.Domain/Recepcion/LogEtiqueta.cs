using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Recepcion
{
    public class LogEtiqueta
    {
        public int Id { get; set; }                     //NU_LOG_ETIQUETA
        public int? Agenda { get; set; }                //NU_AGENDA
        public int? NumeroEtiqueta { get; set; }        //NU_ETIQUETA
        public string CodigoProducto { get; set; }      //CD_PRODUTO
        public decimal? Faixa { get; set; }             //CD_FAIXA
        public int? Empresa { get; set; }               //CD_EMPRESA
        public string Identificador { get; set; }       //NU_IDENTIFICADOR
        public decimal? Cantidad { get; set; }          //QT_MOVIMIENTO
        public string Ubicacion { get; set; }           //CD_ENDERECO
        public DateTime? FechaOperacion { get; set; }   //DT_OPERACION
        public long? NroTransaccion { get; set; }       //NU_TRANSACCION
        public long? NroInterfazEjecucion { get; set; } //NU_INTERFAZ_EJECUCION
        public DateTime? Vencimiento { get; set; }      //DT_FABRICACAO
        public string TipoMovimiento { get; set; }      //TP_MOVIMIENTO
        public string Aplicacion { get; set; }          //CD_APLICACAO
        public int? Funcionario { get; set; }           //CD_FUNCIONARIO
    }
}
