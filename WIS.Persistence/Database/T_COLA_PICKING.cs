using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("T_COLA_PICKING")]
    public partial class T_COLA_PICKING
    {

        [Key]
        public int NU_COLA_PICKING { get; set; }

        public int? USERID { get; set; }

        public int? CD_EQUIPO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public int? NU_PREFERENCIA { get; set; }

    }
}