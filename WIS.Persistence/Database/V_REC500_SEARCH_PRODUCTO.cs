using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    [Table("V_REC500_SEARCH_PRODUCTO")]
    public partial class V_REC500_SEARCH_PRODUCTO
    {
        [StringLength(3)]
        public string CD_CLIENTE { get; set; }
        [Key]
        public int CD_EMPRESA { get; set; }

        [Key]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [StringLength(105)]
        public string VL_CAMPO_BUSQUEDA { get; set; }

    }
}