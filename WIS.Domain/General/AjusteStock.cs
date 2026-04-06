using System;

namespace WIS.Domain.General
{
    public class AjusteStock
    {
        public string Producto { get; set; }                    //CD_PRODUTO
        public decimal Faixa { get; set; }                      //CD_FAIXA
        public string Identificador { get; set; }               //NU_IDENTIFICADOR
        public int Empresa { get; set; }                        //CD_EMPRESA
        public DateTime FechaRealizado { get; set; }            //DT_REALIZADO
        public string TipoAjuste { get; set; }                  //TP_AJUSTE
        public decimal? QtMovimiento { get; set; }              //QT_MOVIMIENTO
        public string DescMotivo { get; set; }                  //DS_MOTIVO
        public string TpDocumento { get; set; }                 //TP_DOCUMENTO
        public string NuDocumento { get; set; }                 //NU_DOCUMENTO
        public DateTime? FechaModificacion { get; set; }        //DT_UPDROW
        public string IdProcesar { get; set; }                  //ID_PROCESAR
        public string CdMotivoAjuste { get; set; }              //CD_MOTIVO_AJUSTE
        public int NuAjusteStock { get; set; }                  //NU_AJUSTE_STOCK
        public int? NuLogInventario { get; set; }               //NU_LOG_INVENTARIO
        public long? NuTransaccion { get; set; }                //NU_TRANSACCION
        public string Predio { get; set; }                      //NU_PREDIO
        public int? FuncionarioMotivo { get; set; }             //CD_FUNC_MOTIVO
        public DateTime? FechaMotivo { get; set; }              //DT_MOTIVO
        public string Aplicacion { get; set; }                  //CD_APLICACAO
        public int? Funcionario { get; set; }                   //CD_FUNCIONARIO
        public string IdAreaAveria { get; set; }                //ID_AREA_AVERIA
        public string Ubicacion { get; set; }                   //CD_ENDERECO
        public decimal? NuInventarioEnderecoDet { get; set; }   //NU_INVENTARIO_ENDERECO_DET
        public string IdProcesado { get; set; }                 //ID_PROCESADO
		public long? NuInterfazEjecucion { get; set; }          //NU_INTERFAZ_EJECUCION
		public string Serializado { get; set; }                 //VL_SERIALIZADO
        public DateTime? FechaVencimiento { get; set; }         //DT_FABRICACAO
        public string Metadata { get; set; }                    //VL_METADATA
		public string Atributos { get; set; }                   //VL_ATRIBUTOS_LPN
    }
}
