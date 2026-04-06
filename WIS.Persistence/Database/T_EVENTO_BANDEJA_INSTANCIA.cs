namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_EVENTO_BANDEJA_INSTANCIA")]
    public partial class T_EVENTO_BANDEJA_INSTANCIA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_EVENTO_BANDEJA_INSTANCIA { get; set; }

        public int NU_EVENTO_BANDEJA { get; set; }

        public int NU_EVENTO_INSTANCIA { get; set; }

        [StringLength(100)]
        [Column]
        public string ND_ESTADO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public virtual T_EVENTO_BANDEJA T_EVENTO_BANDEJA { get; set; }

        public virtual T_EVENTO_INSTANCIA T_EVENTO_INSTANCIA { get; set; }
    }
}
