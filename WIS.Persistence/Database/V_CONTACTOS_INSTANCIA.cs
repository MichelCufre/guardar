namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_CONTACTOS_INSTANCIA")]
    public partial class V_CONTACTOS_INSTANCIA
    {
        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }

        public int CD_EMPRESA { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_CONTACTO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_EMAIL { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_CONTACTO { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_CONTACTO { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_TELEFONO { get; set; }

        public int? USERID { get; set; }

        public int? NU_EVENTO_INSTANCIA { get; set; }
    }
}
