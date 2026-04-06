using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_INV410_INVENTARIO")]
    public partial class V_INV410_INVENTARIO
    {
        [Key]
        public decimal NU_INVENTARIO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_INVENTARIO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CONTROLAR_VENCIMIENTO { get; set; }

        [StringLength(9)]
        [Column]
        public string TP_INVENTARIO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TP_INVENTARIO { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_ESTADO_INVENTARIO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ESTADO_INVENTARIO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_CIERRE_CONTEO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_CIERRE_CONTEO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_BLOQ_USR_CONTEO_SUCESIVO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_MODIFICAR_STOCK_EN_DIF { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PERMITE_INGRESAR_MOTIVO { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CREACION_WEB { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ACTUALIZAR_CONTEO_FIN_AUTO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(1)]
        public string FL_EXCLUIR_SUELTOS { get; set; }

        [StringLength(1)]
        public string FL_EXCLUIR_LPNS { get; set; }

        [StringLength(1)]
        public string FL_RESTABLECER_LPN_FINALIZADO { get; set; }   
        
        public DateTime? DT_UPDROW { get; set; }
        
        [StringLength(1)]
        public string FL_GENERAR_PRIMER_CONTEO { get; set; }

        [StringLength(1)]
        public string FL_PERMITE_ASOC_UBIC_OTRO_INV { get; set; }

    }
}
