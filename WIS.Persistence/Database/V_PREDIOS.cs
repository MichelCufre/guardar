namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PREDIOS")]
    public partial class V_PREDIOS
    {
        [Key]
        [Required]
        [StringLength(20)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_PREDIO { get; set; }
        
        [StringLength(100)]
        [Column]
        public string DS_ENDERECO { get; set; }
        
        [StringLength(20)]
        [Column]
        public string CD_PUNTO_ENTREGA { get; set; }
        
        [Column]
        [StringLength(1)]
        public string FL_SYNC_REALIZADA { get; set; }
        
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }

        [StringLength(50)]
        [Column]
        public string ID_EXTERNO { get; set; }
    }
}
