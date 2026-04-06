using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    [Table("V_FACTURAC_CODIGO_COMP_WFAC250")]
    public partial class V_FACTURAC_CODIGO_COMP_WFAC250
    {
        


        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CD_FACTURACION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string DS_SIGNIFICADO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_CUENTA_CONTABLE { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string DS_CUENTA_CONTABLE { get; set; }


    }
}
