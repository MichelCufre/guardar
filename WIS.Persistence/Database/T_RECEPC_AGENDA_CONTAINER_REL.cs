namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_RECEPC_AGENDA_CONTAINER_REL")]
    public partial class T_RECEPC_AGENDA_CONTAINER_REL
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AGENDA_CONTAINER_REL { get; set; }

        public int NU_AGENDA { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string NU_CONTAINER { get; set; }

        public short NU_SEQ_CONTAINER { get; set; }

        public virtual T_AGENDA T_AGENDA { get; set; }

        public virtual T_CONTAINER T_CONTAINER { get; set; }
    }
}
