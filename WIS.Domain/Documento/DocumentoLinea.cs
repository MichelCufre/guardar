using System;

namespace WIS.Domain.Documento
{
    public class DocumentoLinea
    {
        public int Empresa { get; set; }                        //CD_EMPRESA
        public decimal Faixa { get; set; }                      //CD_FAIXA
        public string Producto { get; set; }                    //CD_PRODUTO
        public short? Situacion { get; set; }                   //CD_SITUACAO
        public string DescripcionProducto { get; set; }         //DS_PRODUTO_INGRESO
        public DateTime? FechaAlta { get; set; }                //DT_ADDROW
        public DateTime? FechaDisponible { get; set; }          //DT_DISPONIBLE
        public DateTime? FechaModificacion { get; set; }        //DT_UPDROW
        public bool Disponible { get; set; }                    //ID_DISPONIBLE
        public string Identificador { get; set; }               //NU_IDENTIFICADOR
        public string IdentificadorIngreso { get; set; }        //NU_INDENTIFICADOR_INGRESO
        public decimal? CantidadDesafectada { get; set; }       //QT_DESAFECTADA
        public decimal? CantidadDescargada { get; set; }        //QT_DESCARGADA
        public decimal? CantidadIngresada { get; set; }         //QT_INGRESADA
        public decimal? CantidadReservada { get; set; }         //QT_RESERVADA
        public decimal? ValorMercaderia { get; set; }           //VL_MERCADERIA
        public decimal? ValorTributo { get; set; }              //VL_TRIBUTO
        public decimal? CIF { get; set; }                       //VL_CIF
        public long NumeroTransaccionDelete { get; set; }       //NU_TRANSACCION_DELETE
        public decimal CantidadDisponible { get; set; }
        public string Documento { get; set; }
        public string TipoDocumento { get; set; }

        public virtual decimal GetCantidadDisponible()
        {
            return (this.CantidadIngresada ?? 0) - (this.CantidadDesafectada ?? 0) - (this.CantidadReservada ?? 0);
        }

        public virtual decimal GetCantidadCambio()
        {
            return (this.CantidadIngresada ?? 0) - (this.CantidadDesafectada ?? 0);
        }

        public virtual void UpdateValue(decimal? quantity, decimal? value)
        {
            this.CantidadIngresada = quantity;
            this.ValorMercaderia = value;
        }

        public virtual void UpdateReserva(decimal? value)
        {
            this.CantidadReservada = this.CantidadReservada - value;
            this.CantidadDesafectada = (this.CantidadDesafectada ?? 0) + value;
        }

        public virtual void DesafectarReserva(decimal? value)
        {
            this.CantidadReservada = this.CantidadReservada - value;
        }

        public virtual void Afectar(decimal? value)
        {
            if (value > 0)
            {
                //Afecto -> controlo disponible
                if (this.GetCantidadDisponible() >= value)
                    this.CantidadReservada = this.CantidadReservada + value;
                else
                    throw new Exception(string.Format("No hay saldo suficiente para realizar la reserva. Producto {0}, lote {1}", this.Producto, this.Identificador));
            }
            else
            {
                //desafecto de la reserva -> controlo reserva
                if (this.CantidadReservada >= (value * -1))
                    this.CantidadReservada = this.CantidadReservada + value;
                else
                    throw new Exception(string.Format("La cantidad a desafectar de la reserva es mayor a la reserva actual. Producto {0}, lote {1}", this.Producto, this.Identificador));
            }
        }

        public virtual DocumentoLinea Clone()
        {
            return new DocumentoLinea()
            {
                CantidadDesafectada = this.CantidadDesafectada,
                CantidadIngresada = this.CantidadIngresada,
                CantidadDescargada = this.CantidadDescargada,
                CantidadReservada = this.CantidadReservada,
                CIF = this.CIF,
                DescripcionProducto = this.DescripcionProducto,
                Disponible = this.Disponible,
                Empresa = this.Empresa,
                Faixa = this.Faixa,
                FechaAlta = this.FechaAlta,
                FechaDisponible = this.FechaDisponible,
                FechaModificacion = this.FechaModificacion,
                Identificador = this.Identificador,
                Producto = this.Producto,
                Situacion = this.Situacion,
                ValorMercaderia = this.ValorMercaderia,
                IdentificadorIngreso = this.IdentificadorIngreso
            };
        }
    }
}
