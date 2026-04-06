namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_CONTACTO_GRU_NS_WEVT030")]
    public partial class V_CONTACTO_GRU_NS_WEVT030
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_CONTACTO { get; set; }

        public int CD_EMPRESA { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_CONTACTO { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_CONTACTO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_EMAIL { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_TELEFONO { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_AGENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        public int? USERID { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
