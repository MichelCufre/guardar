namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_AGENDAS_WREC270")]
    public partial class V_AGENDAS_WREC270
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AGENDA { get; set; }

        [StringLength(20)]
        public string NU_DOCUMENTO { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(10)]
        public string NU_DOCUMENTO_REAL { get; set; }

        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        [StringLength(6)]
        public string ID_ESTADO { get; set; }
    }
}
