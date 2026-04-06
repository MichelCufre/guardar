using System;

namespace WIS.Domain.Picking
{
    public class PedidoAnulado
    {
        public long Id { get; set; }                     //NU_LOG_PEDIDO_ANULADO
        public int? Empresa { get; set; }                //CD_EMPRESA
        public string Cliente { get; set; }              //CD_CLIENTE
        public string Producto { get; set; }             //CD_PRODUTO
        public string Pedido { get; set; }               //NU_PEDIDO
        public decimal Embalaje { get; set; }            //CD_FAIXA
        public string Identificador { get; set; }        //NU_IDENTIFICADOR
        public bool EspecificaIdentificador { get; set; }//ID_ESPECIFICA_IDENTIFICADOR
        public decimal? CantidadAnulada { get; set; }    //QT_ANULADO
        public string Motivo { get; set; }               //DS_MOTIVO
        public int? Funcionario { get; set; }            //CD_FUNCIONARIO
        public string Aplicacion { get; set; }             //CD_APLICACAO
        public DateTime? FechaInsercion { get; set; }    //DT_ADDROW
        public long? InterfazEjecucion { get; set; }     //NU_INTERFAZ_EJECUCION

        public virtual void PrepararInterfaz()
        {
            this.InterfazEjecucion = -1;
        }

        public string EspecificaIdentificadorId { get; set; } //ID_ESPECIFICA_IDENTIFICADOR
    }
}
