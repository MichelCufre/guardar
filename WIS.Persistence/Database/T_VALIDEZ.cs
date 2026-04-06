namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_VALIDEZ")]
    public partial class T_VALIDEZ
    {
        [Key]
        [StringLength(6)]
        [Column]
        public string CD_VALIDEZ { get; set; }

        [StringLength(65)]
        [Column]
        public string DS_VALIDEZ { get; set; }

        public short? QT_DIAS_VALIDADE_LIBERACION { get; set; }

        public short? QT_DIAS_VALIDADE { get; set; }

        public short? QT_DIAS_DURACAO { get; set; }
    }
}
