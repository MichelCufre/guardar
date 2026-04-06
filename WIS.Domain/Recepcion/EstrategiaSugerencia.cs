using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Recepcion
{
    public class EstrategiaSugerencia
    {
        public int NumeroEstrategia { get; set; } //NU_ALM_ESTRATEGIA
        public string CodigoOperativaAsociable { get; set; } //CD_ALM_OPERATIVA_ASOCIABLE
        public string NumeroPredio { get; set; } //NU_PREDIO
        public string CodigoClase { get; set; } //CD_CLASSE
        public string CodigoGrupo { get; set; } //CD_GRUPO
        public int CodigoEmpresa { get; set; } //CD_EMPRESA
        public string CodigoProducto { get; set; } //CD_PRODUTO
        public string Referencia { get; set; } //CD_REFERENCIA
        public string Agrupador { get; set; } //CD_AGRUPADOR
        public int? CodigoInstanciaLogica { get; set; } //NU_ALM_LOGICA_INSTANCIA
        public string UbicacionSugerida { get; set; } //CD_ENDERECO_SUGERIDO
        public decimal Calculo { get; set; } //VL_TIEMPO_CALCULO
        public string Estado { get; set; } //CD_ESTADO
        public string MotivoRechazo { get; set; } //CD_MOTVO_RECHAZO
        public DateTime FechaAdicion { get; set; } //DT_ADDROW
        public DateTime? FechaModificacion { get; set; } //DT_UPDROW
        public int? Funcionario { get; set; }   //CD_FUNCIONARIO
        public long? NuTransaccion { get; set; }    //NU_TRANSACCION
        public string TipoOperativa { get; set; } //TP_ALM_OPERATIVA_ASOCIABLE
        public long NuAlmSugerencia { get; set; }    //NU_ALM_SUGERENCIA
    }
}
