namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_REG010_EQUIPO")]
    public partial class V_REG010_EQUIPO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EQUIPO { get; set; }

        [StringLength(40)]
        [Column]
        public string DS_EQUIPO { get; set; }

        public short CD_FERRAMENTA { get; set; }

        [StringLength(40)]
        [Column]
        public string DS_FERRAMENTA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AUTOASIGNADO { get; set; }

        public short CD_SITUACAO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        [StringLength(30)]
        [Column]
        public string NM_FUNCIONARIO { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_APLICACAO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_REAL { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ZONA { get; set; }

        [StringLength(30)]
        [Column]
        public string TP_OPERATIVA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_OPERATIVA { get; set; }

        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }
    }
}
