namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_EVENTO_GRUPO_INSTANCIA_REL")]
    public partial class T_EVENTO_GRUPO_INSTANCIA_REL
    {
        public int? NU_CONTACTO_GRUPO { get; set; }

        public int NU_EVENTO_INSTANCIA { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_INSTANCIA_GRUPO { get; set; }

        public int? NU_CONTACTO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public virtual T_CONTACTO T_CONTACTO { get; set; }

        public virtual T_CONTACTO_GRUPO T_CONTACTO_GRUPO { get; set; }

        public virtual T_EVENTO_INSTANCIA T_EVENTO_INSTANCIA { get; set; }
    }
}
