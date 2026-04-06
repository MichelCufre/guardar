namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PORTERIA_EGRESOS")]
    public partial class V_PORTERIA_EGRESOS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_CAMION { get; set; }

        [StringLength(50)]
        [Column]
        public string DS_CAMION { get; set; }

        [StringLength(50)]
        [Column]
        public string DS_DOCUMENTO { get; set; }

        [StringLength(15)]
        [Column]
        public string CD_PLACA_CARRO { get; set; }

        public short? CD_PORTA { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
