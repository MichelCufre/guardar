using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Recepcion.Enums;

namespace WIS.Domain.Recepcion
{
    public class ReferenciaRecepcion
    {
        public ReferenciaRecepcion()
        {
            Detalles = new List<ReferenciaRecepcionDetalle>();
        }

        public ReferenciaRecepcion(string codigoAgente, string tipoAgente)
        {
            CodigoAgente = codigoAgente;
            TipoAgente = tipoAgente;
            Detalles = new List<ReferenciaRecepcionDetalle>();
        }

        public int Id { get; set; }                             //NU_RECEPCION_REFERENCIA
        public string Numero { get; set; }                      //NU_REFERENCIA
        public string TipoReferencia { get; set; }              //TP_REFERENCIA
        public string CodigoCliente { get; set; }               //CD_CLIENTE
        public int IdEmpresa { get; set; }                      //CD_EMPRESA
        public string Moneda { get; set; }                      //CD_MONEDA
        public int Situacion { get; set; }                      //CD_SITUACAO
        public string Anexo1 { get; set; }                      //DS_ANEXO1
        public string Anexo2 { get; set; }                      //DS_ANEXO3
        public string Anexo3 { get; set; }                      //DS_ANEXO3
        public string Memo { get; set; }                        //DS_MEMO
        public DateTime? FechaEmitida { get; set; }             //DT_EMITIDA
        public DateTime? FechaEntrega { get; set; }             //DT_ENTREGA
        public DateTime? FechaInsercion { get; set; }           //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }        //DT_UPDROW
        public DateTime? FechaVencimientoOrden { get; set; }    //DT_VENCIMIENTO_ORDEN
        public string IdPredio { get; set; }                    //NU_PREDIO
        public long? NumeroInterfazEjecucion { get; set; }      //NU_INTERFAZ_EJECUCION
        public string Serializado { get; set; }                 //VL_SERIALIZADO
        public string Estado { get; set; }                      //ND_ESTADO_REFERENCIA
        public long? NumeroTransaccion { get; set; }            //NU_TRANSACCION
        public long? NumeroTransaccionDelete { get; set; }      //NU_TRANSACCION_DELETE

        #region WMS_API
        public string CodigoAgente { get; set; }
        public string TipoAgente { get; set; }
        #endregion

        public Empresa Empresa { get; set; }

        public virtual List<ReferenciaRecepcionDetalle> Detalles { get; set; }

        /// <summary>
        /// Comprueba si la referencia está en uso
        /// </summary>
        /// <returns>True si la referencia esta en uso, false de lo contrario</returns>
        public virtual bool EnUso()
        {
            return this.Detalles.Any(rd => (rd.CantidadRecibida != 0 || rd.CantidadAgendada != 0 || rd.CantidadAnulada != 0));
        }


        public override string ToString()
        {
            string memo = this.Memo;

            if (memo?.Length > 100)
                memo = Memo?.Substring(0, 100);

            return $"{Numero} - {memo}";
        }

        /// <summary>
        /// Retora los saldos de los detalles especificados mas los identificadores (AUTO)
        /// </summary>
        /// <param name="idEmpresa"></param>
        /// <param name="producto"></param>
        /// <param name="identificador"></param>
        /// <returns></returns>
        public virtual decimal GetSaldoDetalles(string producto, string identificador)
        {

            return Detalles.Where(d => d.CodigoProducto == producto
                                      && (d.Identificador == identificador || d.Identificador == ManejoIdentificadorDb.IdentificadorAuto))
                           .Sum(s => s.GetSaldo());

        }


    }
}
