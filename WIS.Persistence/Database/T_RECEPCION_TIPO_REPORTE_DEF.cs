using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    public class T_RECEPCION_TIPO_REPORTE_DEF
    {
        [Key]
        [Column(Order = 0)]
        public string TP_RECEPCION { get; set; }
        [Key]
        [Column(Order = 1)]
        public string CD_REPORTE { get; set; }
        public virtual T_RECEPCION_TIPO T_RECEPCION_TIPO { get; set; }
        public virtual T_REPORTE_DEFINICION T_REPORTE_DEFINICION { get; set; }
    }
}
