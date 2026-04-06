namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PORTERIA_ENTRADA_SIN_AGENDA")]
    public partial class V_PORTERIA_ENTRADA_SIN_AGENDA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PORTERIA_VEHICULO { get; set; }

        [Required]
        [StringLength(15)]
        [Column]
        public string ND_TRANSPORTE { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_TRANSPORTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TRANSPORTE { get; set; }

        [StringLength(10)]
        [Column]
        public string VL_MATRICULA_1 { get; set; }

        [StringLength(10)]
        [Column]
        public string VL_MATRICULA_2 { get; set; }

        public decimal? VL_PESO_ENTRADA { get; set; }

        public decimal? VL_PESO_SALIDA { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        public DateTime? DT_PORTERIA_ENTRADA { get; set; }

        public DateTime? DT_PORTERIA_SALIDA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_SOLO_BALANZA { get; set; }

        public int? NU_EJECUCION_ENTRADA { get; set; }

        public int? NU_EJECUCION_SALIDA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_SALIDA_HABILITADA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CONTROL_SALIDA { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_CLIENTE { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_SECTOR { get; set; }

        [StringLength(300)]
        [Column]
        public string DS_SECTOR { get; set; }

        [StringLength(15)]
        [Column]
        public string ND_POTERIA_MOTIVO { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_MOTIVO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_MOTIVO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_AGENDAS_ASOCIADAS { get; set; }
    }
}
