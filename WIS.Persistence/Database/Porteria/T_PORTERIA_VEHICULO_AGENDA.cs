namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("T_PORTERIA_VEHICULO_AGENDA")]
    public partial class T_PORTERIA_VEHICULO_AGENDA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PORTERIA_VEHICULO_AGENDA { get; set; }

        public int NU_PORTERIA_VEHICULO { get; set; }

        public int NU_AGENDA { get; set; }
    }
}
