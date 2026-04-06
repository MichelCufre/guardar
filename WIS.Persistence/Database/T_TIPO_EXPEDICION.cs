namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_TIPO_EXPEDICION")]
    public partial class T_TIPO_EXPEDICION
    {
        [Key]
        [StringLength(6)]
        [Column]
        public string TP_EXPEDICION { get; set; }

        [StringLength(60)]
        [Column]
        public string NM_EXPEDICION { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_REQUIERE_BOX { get; set; }

        [StringLength(1)]
        [Column]
        public string TP_ARMADO_EGRESO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_FACTURAR_EN_EMPAQUETADO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CIERRE_CAMION_EN_BOX { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_EXPEDIR_CONTENEDOR { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_EMPAQUETA_CONTENEDOR { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_TRASPASO_EMPRESA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PRODUCCION { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CONSUMO_INTERNO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PERMITE_FACTURACION_PARCIAL { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CIERRE_CAMION_EN_EMPAQUE { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_FACTURA_AUTO_COMPLETAR { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PERMITE_ANULACION_PARCIAL { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_MODALIDAD_EMPAQUETADO { get; set; }

        [Required]
        [StringLength(6)]
        [Column]
        public string CD_GRUPO_EXPEDICION { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_DISPONIBLE { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ANULA_PENDIENTES_AL_LIBERAR { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_EXPEDIR_COMPLETO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PERMITE_FACT_SIN_PRECINTO { get; set; }

        public short? VL_CANTIDAD_PRECINTOS { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_FACTURACION_REQUERIDA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_FACTURACION_PARCIAL { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_FACTURACION_POR_PEDIDO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CONTROLAR_UND_BULTO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_REQUIERE_LIBERACION_TOTAL { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_TRACKING { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PREPARABLE { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_EMPAQUETA_SOLO_UN_PRODUCTO { get; set; }
    }
}
