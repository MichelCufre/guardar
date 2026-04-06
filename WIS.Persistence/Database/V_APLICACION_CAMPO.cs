using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_APLICACION_CAMPO")]
    public partial class V_APLICACION_CAMPO
    {

        [Key]
        [Column(Order = 0)]
        [StringLength(30)]
        public string CD_APLICACION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(30)]
        public string CD_CAMPO { get; set; }

        [StringLength(100)]
        public string DS_CAMPO { get; set; }

        [StringLength(1)]
        public string FL_CODIGO_MULTIDATO { get; set; }

    }
}
