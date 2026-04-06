using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Recepcion.Enums;
using WIS.Domain.Recepcion.RecepcionAgendamiento;

namespace WIS.Domain.Recepcion
{
    public class AgendaDetalle
    {
        public int IdAgenda { get; set; }                       //NU_AGENDA
        public int IdEmpresa { get; set; }                      //CD_EMPRESA
        public string CodigoProducto { get; set; }              //CD_PRODUTO
        public decimal Faixa { get; set; }                      //CD_FAIXA
        public string Identificador { get; set; }               //NU_IDENTIFICADOR
        public EstadoAgendaDetalle Estado { get; set; }         //CD_SITUACAO
        public decimal CantidadAgendada { get; set; }           //QT_AGENDADO
        public decimal CantidadRecibida { get; set; }           //QT_RECIBIDA
        public decimal CantidadAceptada { get; set; }           //QT_ACEPTADA
        public decimal CantidadAgendadaOriginal { get; set; }   //QT_AGENDADO_ORIGINAL
        public decimal CantidadRecibidaFicticia { get; set; }   //QT_RECIBIDA_FICTICIA
        public decimal CantidadCrossDocking { get; set; }       //QT_CROSS_DOCKING
        public DateTime? Vencimiento { get; set; }              //DT_FABRICACAO
        public DateTime? FechaAlta { get; set; }                //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }        //DT_UPDROW
        public DateTime? FechaAceptacionRecepcion { get; set; } //DT_ACEPTADA_RECEPCION
        public int? IdUsuarioAceptacionRecepcion { get; set; }  //CD_FUNC_ACEPTO_RECEPCION
        public decimal? CIF { get; set; }                       //VL_CIF
        public decimal? Precio { get; set; }                    //VL_PRECIO
        public long? NumeroTransaccion { get; set; }            //NU_TRANSACCION

        #region WMS_API                                              
        public short EstadoId { get; set; }                   //CD_SITUACAO
        public int DetalleReferenciaId { get; set; }          //NU_RECEPCION_REFERENCIA_DET

        #endregion

        public Agenda Agenda { get; set; }

        public List<DetalleAgendaReferenciaAsociada> AsociacionesDetalleReferencia { get; set; }
        public List<AgendaDetalleProblema> ProblemasRecepcion { get; set; }

        public AgendaDetalle()
        {
            AsociacionesDetalleReferencia = new List<DetalleAgendaReferenciaAsociada>();
            ProblemasRecepcion = new List<AgendaDetalleProblema>();
        }

        public virtual string GetCompositeId()
        {
            return $"{IdAgenda}#{CodigoProducto}#{Identificador}#{Faixa.ToString("#.###")}#{IdEmpresa}";
        }

        /// <summary>
        /// Comprueba si los problemas de recepcion estan aceptados para cambiar el estado a SIN DIFERENCIAS
        /// </summary>
        public virtual void ComprobarCambioEstadoSinDiferencia()
        {
            if (!ProblemasRecepcion.Any(s => !s.Aceptado))
                this.Estado = EstadoAgendaDetalle.ConferidaSinDiferencias;
        }
        public virtual AgendaDetalleProblema GetProblemaRecepcion(int idProblema)
        {
            return ProblemasRecepcion.FirstOrDefault(s => s.Id == idProblema);
        }
        public virtual bool TieneProblemasSinResolver()
        {
            return ProblemasRecepcion.Any(s => !s.Aceptado);
        }

        public virtual decimal? GetCantDisponibleIngresada()
        {
            return this.CantidadAgendada - (this.CantidadRecibida );
        }

        public virtual decimal GetSaldo()
        {
            decimal? saldo;

            if ((this.CantidadAgendada ) > (this.CantidadCrossDocking ))
            {
                saldo = this.CantidadAgendada - (this.CantidadRecibida );
            }
            else
            {
                saldo = this.CantidadAgendada - (this.CantidadCrossDocking );
            }

            return (saldo ?? 0);
        }
    }
}
