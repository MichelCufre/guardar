using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_PEDIDO_SAIDA_KIT130")]
    public class V_PEDIDO_SAIDA_KIT130
    {
        [Key]
        public string NU_PEDIDO { get; set; }
        [Key]
        public int CD_EMPRESA { get; set; }
        [Key]
        public string CD_CLIENTE { get; set; }
        public string NU_PRDC_INGRESO { get; set; }
        public int? NU_PREPARACION_MANUAL { get; set; }
        public int? CD_ROTA { get; set; }
        public short? CD_SITUACAO { get; set; }
        public DateTime? DT_LIBERAR_DESDE { get; set; }
        public DateTime? DT_LIBERAR_HASTA { get; set; }
        public DateTime? DT_ENTREGA { get; set; }
        public string ID_MANUAL { get; set; }
        public string ID_AGRUPACION { get; set; }
        public DateTime? DT_EMITIDO { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        public short? NU_ORDEN_LIBERACION { get; set; }
        public int? NU_ULT_PREPARACION { get; set; }
        public DateTime? DT_ULT_PREPARACION { get; set; }
        public string DS_MEMO { get; set; }
        public string CD_ORIGEN { get; set; }
        public string NM_EMPRESA { get; set; }
        public string CD_AGENTE { get; set; }
        public string TP_AGENTE { get; set; }
        public string DS_CLIENTE { get; set; }
        public string DS_SITUACAO { get; set; }
        public string DS_ROTA { get; set; }
    }
}
