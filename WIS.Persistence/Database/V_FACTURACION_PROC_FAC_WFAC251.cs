using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    [Table("V_FACTURACION_PROC_FAC_WFAC251")]
    public partial class V_FACTURACION_PROC_FAC_WFAC251
    {

        [Required]
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CD_PROCESO { get; set; }

        
        [Column(Order = 1)]
        [StringLength(200)]
        public string DS_PROCESO { get; set; }
                
        public short CD_SITUACAO_ERROR { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        
        [StringLength(20)]
        [Column]
        public string CD_FACTURACION { get; set; }

       
        [StringLength(30)]
        [Column]
        public string NM_PROCEDIMIENTO { get; set; }

        
        [StringLength(20)]
        [Column]
        public string NU_COMPONENTE { get; set; }

       
        [StringLength(10)]
        [Column]
        public string NU_CUENTA_CONTABLE { get; set; }

        
        [StringLength(100)]
        [Column]
        public string DS_CUENTA_CONTABLE { get; set; }

       
        [StringLength(1)]
        [Column]
        public string TP_PROCESO { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string DS_SIGNIFICADO { get; set; }

        
        [StringLength(1)]
        [Column]
        public string FL_EJEC_POR_HORA { get; set; }






    }
}
