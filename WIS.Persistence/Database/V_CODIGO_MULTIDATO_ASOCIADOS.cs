using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WIS.Persistence.Database;


namespace WIS.Persistence.Database
{

    [Table("V_CODIGO_MULTIDATO_ASOCIADOS")]
    public partial class V_CODIGO_MULTIDATO_ASOCIADOS
    {

        [Key]
        [Column(Order = 0)]
        [StringLength(30)]
        public string CD_CODIGO_MULTIDATO { get; set; }

        [StringLength(100)]
        public string DS_CODIGO_MULTIDATO { get; set; }

        [Key]
        [Column(Order = 2)]
        public int CD_EMPRESA { get; set; }


        [StringLength(1)]
        public string FL_HABILITADO { get; set; }

        public string FL_ASOCIADO { get; set; }

    }
}
