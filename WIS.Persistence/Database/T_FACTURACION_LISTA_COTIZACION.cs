namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_FACTURACION_LISTA_COTIZACION")]
    public partial class T_FACTURACION_LISTA_COTIZACION
    {
        [Key]
        [StringLength(50)]
        public string CD_FACTURACION { get; set; }

        [Key]
        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }

        [Key]
        public int CD_LISTA_PRECIO { get; set; }
        public int? CD_FUNCIONARIO { get; set; }

        public decimal? QT_IMPORTE { get; set; }
        public decimal? QT_IMPORTE_MINIMO { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        public DateTime? DT_ADDROW { get; set; }
    }
}

