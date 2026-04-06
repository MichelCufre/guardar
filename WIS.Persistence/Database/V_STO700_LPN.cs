namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_STO700_LPN")]
    public partial class V_STO700_LPN
    {
        public int CD_EMPRESA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ESTADO { get; set; }

        [StringLength(400)]
        [Column]
        public string DS_LPN_TIPO { get; set; }

        public DateTime? DT_ACTIVACION { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_FIN { get; set; }

        [StringLength(50)]
        [Column]
        public string ID_LPN_EXTERNO { get; set; }

        [StringLength(55)]
        [Column]
        public string ID_PACKING { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(6)]
        [Column]
        public string ID_ESTADO { get; set; }

        [Key]
        public long NU_LPN { get; set; }

        public long? NU_TRANSACCION { get; set; }

        [StringLength(10)]
        [Column]
        public string TP_LPN_TIPO { get; set; }

        public int? NU_AGENDA { get; set; }
        public DateTime? DT_UPDROW { get; set; }

    }
}