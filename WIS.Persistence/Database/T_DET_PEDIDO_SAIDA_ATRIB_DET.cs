namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_DET_PEDIDO_SAIDA_ATRIB_DET")]
    public partial class T_DET_PEDIDO_SAIDA_ATRIB_DET
    {
        [Key]
        [Column(Order = 0)]
        public long NU_DET_PED_SAI_ATRIB { get; set; }

        [Key]
        [Column(Order = 1)]
        public int ID_ATRIBUTO { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(1)]
        public string FL_CABEZAL { get; set; }

        [StringLength(400)]
        public string VL_ATRIBUTO { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
