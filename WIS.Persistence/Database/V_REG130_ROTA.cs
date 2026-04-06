namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_REG130_ROTA")]
    public partial class V_REG130_ROTA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CD_ROTA { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_ROTA { get; set; }

        public short? CD_PORTA { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_PORTA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        public short? CD_ONDA { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_ONDA { get; set; }

        public int? CD_TRANSPORTADORA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TRANSPORTADORA { get; set; }

        [StringLength(1)]
        [Column]
        public string ACTIVO { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        public DateTime? DT_SITUACAO { get; set; }

        public DateTime? DT_ALTERACAO { get; set; }

        public DateTime? DT_CADASTRAMENTO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_ORDEM_CARGA { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_ZONA { get; set; }
    }
}
