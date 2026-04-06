using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("T_CODIGO_MULTIDATO_EMPRESA")]
    public partial class T_CODIGO_MULTIDATO_EMPRESA
    {

        [Key]
        [Column(Order = 0)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(30)]
        public string CD_CODIGO_MULTIDATO { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_HABILITADO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

    }
}
