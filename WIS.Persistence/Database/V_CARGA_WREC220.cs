namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_CARGA_WREC220")]
    public partial class V_CARGA_WREC220
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_CARGA { get; set; }

        public short? CD_ROTA { get; set; }

        [StringLength(30)]
        public string DS_ROTA { get; set; }

        [StringLength(2)]
        public string NU_PREDIO { get; set; }
    }
}
