namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("T_PORTERIA_VEHICULO_CAMION")]
    public partial class T_PORTERIA_VEHICULO_CAMION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PORTERIA_VEHICULO_CAMION { get; set; }

        public int NU_PORTERIA_VEHICULO { get; set; }

        public int CD_CAMION { get; set; }
    }
}
