using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.StockEntities
{
    public class StockSuelto
    {
        public string Ubicacion { get; set; }                   //CD_ENDERECO
        public int Empresa { get; set; }                        //CD_EMPRESA
        public string Producto { get; set; }                    //CD_PRODUTO
        public decimal Faixa { get; set; }                      //CD_FAIXA
        public string Identificador { get; set; }               //NU_IDENTIFICADOR
        public DateTime? Vencimiento { get; set; }              //DT_FABRICACAO
        public decimal? Cantidad { get; set; }                  //QT_ESTOQUE
        public decimal? ReservaSalida { get; set; }             //QT_RESERVA_SAIDA
        public decimal? CantidadTransitoEntrada { get; set; }   //QT_TRANSITO_ENTRADA
        public decimal? CantidadLpn { get; set; }               //QT_ESTOQUE_LPN
        public decimal? CantidadSuelta { get; set; }            //QT_ESTOQUE_SUELTO
        public string Averia { get; set; }                      //ID_AVERIA
        public string Inventario { get; set; }                  //ID_INVENTARIO
        public string ControlCalidad { get; set; }              //ID_CTRL_CALIDAD
        public string MotivoAveria { get; set; }                //CD_MOTIVO_AVERIA
    }
}