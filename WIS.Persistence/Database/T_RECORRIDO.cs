using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("T_RECORRIDO")]
    public partial class T_RECORRIDO
    {

        [Key]
        [Column(Order = 0)]
        public int NU_RECORRIDO { get; set; }

        [Required]
        [StringLength(50)]
        public string NM_RECORRIDO { get; set; }

        [Required]
        [StringLength(200)]
        public string DS_RECORRIDO { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_DEFAULT { get; set; }

        [Required]
        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        [StringLength(1)]
        [Required]
        public string FL_HABILITADO { get; set; }

        public DateTime? DT_ADDROW { get; set; }    
        
        public DateTime? DT_UPDROW { get; set; }    
    }
}
