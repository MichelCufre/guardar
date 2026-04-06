using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    public class V_FACTURAC_CUENTA_CONT_WFAC405
    {
        [Key]
        [StringLength(10)]
        public string NU_CUENTA_CONTABLE { get; set; }

        [StringLength(100)]
        public string DS_CUENTA_CONTABLE { get; set; }
    }
}