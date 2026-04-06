namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_EQUIPO")]
    public partial class T_EQUIPO
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
        public string CD_ENDERECO { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_REAL { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_ZONA { get; set; }

        [StringLength(30)]
        [Column]
        public string TP_OPERATIVA { get; set; }

        public string NU_COMPONENTE { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_APLICACAO { get; set; }
        public virtual T_FERRAMENTAS T_FERRAMENTAS { get; set; }
        public virtual T_ENDERECO_ESTOQUE T_ENDERECO_ESTOQUE { get; set; }



    }
}
