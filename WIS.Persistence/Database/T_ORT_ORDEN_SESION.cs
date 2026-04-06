
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WIS.Persistence.Database;


namespace WIS.Persistence.Database
{

    [Table("T_ORT_ORDEN_SESION")]
    public partial class T_ORT_ORDEN_SESION
    {

        public long NU_ORT_ORDEN_SESION { get; set; }

        public int NU_ORT_ORDEN { get; set; }

        public int CD_FUNCIONARIO { get; set; }

        public DateTime DT_INICIO { get; set; }

        public DateTime? DT_FIN { get; set; }

    }
}
