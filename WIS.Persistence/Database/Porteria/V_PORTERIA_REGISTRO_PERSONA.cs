namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PORTERIA_REGISTRO_PERSONA")]
    public partial class V_PORTERIA_REGISTRO_PERSONA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_POTERIA_PERSONA { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string NU_DOCUMENTO { get; set; }

        [Required]
        [StringLength(15)]
        [Column]
        public string ND_TP_DOCUMENTO { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_TP_DOCUMENTO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TP_DOCUMENTO { get; set; }

        [StringLength(2)]
        [Column]
        public string CD_PAIS_EMISOR { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string NM_PERSONA { get; set; }

        [StringLength(100)]
        [Column]
        public string AP_PERSONA { get; set; }

        [StringLength(15)]
        [Column]
        public string NU_CELULAR { get; set; }

        [Required]
        [StringLength(15)]
        [Column]
        public string ND_TP_PERSONA { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_TP_PERSON { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TP_PERSON { get; set; }

        [StringLength(15)]
        [Column]
        public string ND_PUESTO_FUNCIONARIO { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_PUESTO_FUNCIONARIO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_PUESTO_FUNCIONARIO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(100)]
        [Column]
        public string CD_PORTERIA_EMPRESA { get; set; }
    }
}
