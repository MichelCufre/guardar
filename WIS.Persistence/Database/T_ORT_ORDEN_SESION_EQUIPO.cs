
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WIS.Persistence.Database;


namespace WIS.Persistence.Database
{

    [Table("T_ORT_ORDEN_SESION_EQUIPO")]
    public partial class T_ORT_ORDEN_SESION_EQUIPO
    {

        public long NU_ORT_ORDEN_SESION_EQUIPO { get; set; }

        public long NU_ORT_ORDEN_SESION { get; set; }

        public int CD_EQUIPO { get; set; }

        public DateTime DT_INICIO { get; set; }

        public DateTime? DT_FIN { get; set; }

    }
}
