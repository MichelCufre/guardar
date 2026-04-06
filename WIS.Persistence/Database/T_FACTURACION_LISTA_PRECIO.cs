namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_FACTURACION_LISTA_PRECIO")]
    public partial class T_FACTURACION_LISTA_PRECIO
    {
        [Key]
        [StringLength(6)]
        public int CD_LISTA_PRECIO { get; set; }

        [StringLength(50)]
        public string DS_LISTA_PRECIO { get; set; }

        [StringLength(15)]
        public string CD_MONEDA { get; set; }
        
        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }
    }
}

