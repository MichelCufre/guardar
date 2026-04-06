namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("T_PORTERIA_REGISTRO_PERSONA")]
    public partial class T_PORTERIA_REGISTRO_PERSONA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PORTERIA_REGISTRO_PERSONA { get; set; }

        [Required]
        [StringLength(15)]
        [Column]
        public string ND_TP_POTERIA_REGISTRO { get; set; }

        [StringLength(15)]
        [Column]
        public string ND_POTERIA_MOTIVO { get; set; }

        public int CD_FUNCIONARIO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        public int? NU_POTERIA_PERSONA { get; set; }

        public DateTime? DT_PERSONA_ENTRADA { get; set; }

        public DateTime? DT_PERSONA_SALIDA { get; set; }

        public int? NU_PORTERIA_VEHICULO_ENTRADA { get; set; }

        public int? NU_PORTERIA_VEHICULO_SALIDA { get; set; }

        [Required]
        [StringLength(15)]
        [Column]
        public string ND_SECTOR { get; set; }

        [StringLength(400)]
        [Column]
        public string DS_NOTA { get; set; }

        [Required]
        [StringLength(15)]
        [Column]
        public string ND_ESTADO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_SECTOR { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }
    }
}
