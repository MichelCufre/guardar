using System.Collections.Generic;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class LpnResponse
    {
        public long Numero { get; set; }                //NU_LPN
        public string IdExterno { get; set; }           //ID_LPN_EXTERNO
        public int Empresa { get; set; }                //CD_EMPRESA
        public string Tipo { get; set; }                //TP_LPN_TIPO
        public string Estado { get; set; }              //ID_ESTADO
        public string Ubicacion { get; set; }           //CD_ENDERECO
        public string FechaInsercion { get; set; }      //DT_ADDROW
        public string FechaModificacion { get; set; }   //DT_UPDROW
        public string FechaActivacion { get; set; }     //DT_ACTIVACION
        public string FechaFin { get; set; }            //DT_FIN
        public string IdPacking { get; set; }           //ID_PACKING
        public int? NroAgenda { get; set; }             //NU_AGENDA

        public List<LpnDetalleResponse> Detalles { get; set; }

        public LpnResponse()
        {
            Detalles = new List<LpnDetalleResponse>();
        }
    }

    public class LpnDetalleResponse
    {
        public int Id { get; set; }                         //ID_LPN_DET
        public long Numero { get; set; }                    //NU_LPN
        public string CodigoProducto { get; set; }          //CD_PRODUTO
        public decimal Faixa { get; set; }                  //CD_FAIXA
        public int Empresa { get; set; }                    //CD_EMPRESA
        public string Lote { get; set; }                    //NU_IDENTIFICADOR
        public decimal Cantidad { get; set; }               //QT_ESTOQUE
        public string Vencimiento { get; set; }             //DT_FABRICACAO
        public decimal? CantidadDeclarada { get; set; }     //QT_DECLARADA
        public decimal? CantidadRecibida { get; set; }      //QT_RECIBIDA
        public decimal? CantidadReserva { get; set; }       //QT_RESERVA_SAIDA
        public decimal? CantidadExpedida { get; set; }      //QT_EXPEDIDA
        public string IdLineaSistemaExterno { get; set; }   //ID_LINEA_SISTEMA_EXTERNO
        public bool Averiado { get; set; }                  //ID_AVERIA
        public string MotivoAveria { get; set; }            //T_DET_DOMINIO.DS_DOMINIO_VALOR
        public bool ControlDeCalidadPendiente { get; set; } //ID_CTR_CALIDAD

    }
}
