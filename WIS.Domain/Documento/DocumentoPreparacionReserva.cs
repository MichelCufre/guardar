using System;

namespace WIS.Domain.Documento
{
    public class DocumentoPreparacionReserva
    {
        public string NroDocumento { get; set; }                //NU_DOCUMENTO
        public string TipoDocumento { get; set; }               //TP_DOCUMENTO
        public int Preparacion { get; set; }                    //NU_PREPARACION
        public int Empresa { get; set; }                        //CD_EMPRESA
        public string Producto { get; set; }                    //CD_PRODUTO
        public decimal Faixa { get; set; }                      //CD_FAIXA
        public string Identificador { get; set; }               //NU_IDENTIFICADOR
        public bool EspecificaIdentificador { get; set; }       //ID_ESPECIFICA_IDENTIFICADOR
        public string NroIdentificadorPicking { get; set; }     //NU_IDENTIFICADOR_PICKING_DET
        public decimal? CantidadProducto { get; set; }          //QT_PRODUTO
        public decimal? CantidadPreparada { get; set; }         //QT_PREPARADO
        public decimal? CantidadAnular { get; set; }            //QT_ANULAR
        public DateTime? FechaAlta { get; set; }                //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }        //DT_UPDROW

        public IDocumento DocumentoIngreso { get; set; }
        public string Semiacabado { get; set; }
        public long? NumeroTransaccion { get; set; }
        public long? NumeroTransaccionDelete { get; set; }

        #region WMS_API
        public string EspecificaIdentificadorId { get; set; }
        public string Auditoria { get; set; }
        public int Secuencia { get;  set; }

        #endregion

        public virtual void AfectarCantidadProducto(decimal? value)
        {
            if (value > 0)
            {
                this.CantidadProducto = this.CantidadProducto + value;
                this.CantidadPreparada = this.CantidadPreparada + value;
            }
            else
            {
                //desafecto de la reserva -> controlo reserva
                if (this.CantidadProducto >= (value * -1))
                {
                    this.CantidadProducto = this.CantidadProducto + value;
                    this.CantidadPreparada = Math.Max(0, (this.CantidadPreparada + value) ?? 0);
                }
                else
                    throw new Exception("La cantidad a desafectar de la reserva es mayor a la reserva actual.");
            }
        }

        public virtual void AnularReserva(decimal? value)
        {
            if (CantidadDisponible() >= -value)
            {
                this.CantidadProducto = this.CantidadProducto + value;
            }
            else
                throw new Exception("La cantidad a anular de la reserva es mayor a la reserva actual.");
        }

        public virtual decimal CantidadDisponible()
        {
            return (this.CantidadProducto ?? 0) - (this.CantidadPreparada ?? 0);
        }
    }

}
