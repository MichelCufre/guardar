namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_INTERFAZ_EJEC_DATA")]
    public partial class V_INTERFAZ_EJEC_DATA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }
        public short? CD_SITUACAO { get; set; }
        public int? CD_INTERFAZ_EXTERNA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_INTERFAZ_EXTERNA { get; set; }

        [Column]
        public DateTime DT_ADDROW { get; set; }

        [Column]
        public byte[] DATA { get; set; }
    }
}
