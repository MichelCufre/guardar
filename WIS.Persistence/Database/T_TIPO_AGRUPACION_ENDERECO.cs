
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("T_TIPO_AGRUPACION_ENDERECO")]
    public partial class T_TIPO_AGRUPACION_ENDERECO
    {

        [Key]
        [Column(Order = 0)]
        public int TP_AGRUPACION_UBIC { get; set; }

        [Required]
        [StringLength(200)]
        public string DS_AGRUPACION_UBIC { get; set; }

        [StringLength(1)]
        public string FL_EMPRESA { get; set; }

        [StringLength(1)]
        public string FL_PRODUTO { get; set; }

        [StringLength(1)]
        public string FL_PRODUCTO_FAMILIA { get; set; }

        [StringLength(1)]
        public string FL_PRODUCTO_RAMO { get; set; }

        [StringLength(1)]
        public string FL_PRODUCTO_ROTATIVIDADE { get; set; }

        [StringLength(1)]
        public string FL_PRODUCTO_CLASSE { get; set; }

        [StringLength(1)]
        public string FL_PRODUCTO_SUB_CLASSE { get; set; }

        [StringLength(1)]
        public string FL_PRODUCTO_ANEXO1 { get; set; }

        [StringLength(1)]
        public string FL_PRODUCTO_ANEXO2 { get; set; }

        [StringLength(1)]
        public string FL_PRODUCTO_ANEXO3 { get; set; }

        [StringLength(1)]
        public string FL_PRODUCTO_ANEXO4 { get; set; }

        [StringLength(1)]
        public string FL_PRODUCTO_ANEXO5 { get; set; }

        [StringLength(1)]
        public string FL_CLIENTE { get; set; }

        [StringLength(1)]
        public string FL_COMPARTE_PICKING { get; set; }

        [StringLength(1)]
        public string FL_COMPARTE_ENTREGA { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

    }
}
