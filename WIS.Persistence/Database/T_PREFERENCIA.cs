namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PREFERENCIA")]
    public partial class T_PREFERENCIA
    {
        [Key]
        public int NU_PREFERENCIA { get; set; }

        [StringLength(200)]
        public string DS_PREFERENCIA { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [StringLength(10)]
        public string ID_BLOQUE_MIN { get; set; }

        [StringLength(10)]
        public string ID_BLOQUE_MAX { get; set; }

        [StringLength(10)]
        public string ID_CALLE_MIN { get; set; }

        [StringLength(10)]
        public string ID_CALLE_MAX { get; set; }

        public int? NU_COLUMNA_MIN { get; set; }

        public int? NU_COLUMNA_MAX { get; set; }

        public int? NU_ALTURA_MIN { get; set; }

        public int? NU_ALTURA_MAX { get; set; }

        public decimal? PS_BRUTO_MAXIMO { get; set; }

        public decimal? VL_CUBAGEM_MAXIMO { get; set; }

        public int? QT_CLIENTES { get; set; }

        public int? QT_PEDIDOS { get; set; }

        public int? QT_MAXIMO_PICKEOS { get; set; }

        public int? QT_MAXIMO_UNIDADES { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(1)]
        public string FL_HABILITADO_EMPRESA { get; set; }

        [StringLength(1)]
        public string FL_HABILITADO_CLIENTE { get; set; }

        [StringLength(1)]
        public string FL_HABILITADO_RUTA { get; set; }

        [StringLength(1)]
        public string FL_HABILITADO_ZONA { get; set; }

        [StringLength(1)]
        public string FL_HABILITADO_COND_LIBERACION { get; set; }

        [StringLength(1)]
        public string FL_HABILITADO_TP_PEDIDO { get; set; }

        [StringLength(1)]
        public string FL_HABILITADO_TP_EXPEDICION { get; set; }

        [StringLength(1)]
        public string FL_HABILITADO_CLASE { get; set; }

        [StringLength(1)]
        public string FL_HABILITADO_FAMILIA { get; set; }

        [StringLength(1)]
        public string FL_HABILITADO_CONT_ACCESO { get; set; }

        [StringLength(1)]
        public string FL_HABILITADO_RANGO_UBIC { get; set; }

        [StringLength(1)]
        public string FL_HABILITADO_PEDIDO_COMPLETO { get; set; }

        [StringLength(1)]
        public string FL_HABILITADO_LIB_COMPLETO { get; set; }
    }
}
