using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public partial class T_PRDC_INGRESO_DOCUMENTO
    {
        [Key]
        [Column(Order = 0)]
        public string NU_DOCUMENTO_EGR { get; set; }
        [Key]
        [Column(Order = 1)]
        public string TP_DOCUMENTO_EGR { get; set; }
        [Key]
        [Column(Order = 2)]
        public string NU_DOCUMENTO_ING { get; set; }
        [Key]
        [Column(Order = 3)]
        public string TP_DOCUMENTO_ING { get; set; }
        [Key]
        [Column(Order = 4)]
        public string NU_PRDC_INGRESO { get; set; }

        public virtual T_PRDC_INGRESO T_PRDC_INGRESO { get; set; }
    }
}
