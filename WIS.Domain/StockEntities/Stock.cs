using System;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.StockEntities
{
    public class Stock
    {
        public string Ubicacion { get; set; }                   //CD_ENDERECO
        public int Empresa { get; set; }                        //CD_EMPRESA
        public string Producto { get; set; }                    //CD_PRODUTO
        public decimal Faixa { get; set; }                      //CD_FAIXA
        public string Identificador { get; set; }               //NU_IDENTIFICADOR
        public decimal? Cantidad { get; set; }                  //QT_ESTOQUE
        public decimal? ReservaSalida { get; set; }             //QT_RESERVA_SAIDA
        public decimal? CantidadTransitoEntrada { get; set; }   //QT_TRANSITO_ENTRADA
        public DateTime? Vencimiento { get; set; }              //DT_FABRICACAO
        public string Averia { get; set; }                      //ID_AVERIA
        public string MotivoAveria { get; set; }                //CD_MOTIVO_AVERIA
        public string Inventario { get; set; }                  //ID_INVENTARIO
        public DateTime? FechaInventario { get; set; }          //DT_INVENTARIO
        public string ControlCalidad { get; set; }              //ID_CTRL_CALIDAD
        public DateTime? FechaModificacion { get; set; }        //DT_UPDROW
        public long? NumeroTransaccion { get; set; }            //NU_TRANSACCION
        public long? NumeroTransaccionDelete { get; set; }      //NU_TRANSACCION_DELETE

        #region Api
        public decimal? CantidadLpn { get; set; }               //QT_ESTOQUE_LPN
        public decimal? CantidadReservaLpn { get; set; }        //QT_RESERVA_LPN
        public decimal? CantidadDisponibleLpn { get; set; }     //QT_DISPONIBLE_LPN

        public string Predio { get; set; }

        public decimal? CantidadAjustar { get; set; }           //PRD111

        #endregion

        public virtual bool IsUbicacionVacia()
        {
            return 0 == (Cantidad ?? 0) + (ReservaSalida ?? 0) + (CantidadTransitoEntrada ?? 0) && (Inventario == null || Inventario == "R");
        }
        public virtual decimal CantidadDisponible()
        {
            return (Cantidad ?? 0) - (ReservaSalida ?? 0);
        }

        public virtual void SetControlado()
        {
            ControlCalidad = EstadoControlCalidad.Controlado;
        }

        public virtual void EliminarReserva()
        {
            ReservaSalida = 0;
            FechaModificacion = DateTime.Now;
        }

        public virtual string GetLockId(IFormatProvider formatProvider)
        {
            return $"{Ubicacion}#{Empresa}#{Producto}#{Faixa.ToString(formatProvider)}#{Identificador}";
        }
    }
}
