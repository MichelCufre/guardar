using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_INVENTARIO")]
    public partial class T_INVENTARIO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_INVENTARIO()
        {
            T_INVENTARIO_ENDERECO = new HashSet<T_INVENTARIO_ENDERECO>();
        }

        [Key]
        public decimal NU_INVENTARIO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_INVENTARIO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CONTROLAR_VENCIMIENTO { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_ESTADO_INVENTARIO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_CIERRE_CONTEO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_BLOQ_USR_CONTEO_SUCESIVO { get; set; }

        public int? CD_EMPRESA { get; set; }

        public decimal? NU_CONTEO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_SOLO_REGISTROS_FOTO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PERMITE_INGRESAR_MOTIVO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_MODIFICAR_STOCK_EN_DIF { get; set; }

        [StringLength(9)]
        [Column]
        public string TP_INVENTARIO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CREACION_WEB { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ACTUALIZAR_CONTEO_FIN_AUTO { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(1)]
        public string FL_EXCLUIR_SUELTOS { get; set; }

        [StringLength(1)]
        public string FL_EXCLUIR_LPNS { get; set; }        

        [StringLength(1)]
        public string FL_RESTABLECER_LPN_FINALIZADO { get; set; }

        [StringLength(1)]
        public string FL_GENERAR_PRIMER_CONTEO { get; set; }

        [StringLength(1)]
        public string FL_PERMITE_ASOC_UBIC_OTRO_INV { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_INVENTARIO_ENDERECO> T_INVENTARIO_ENDERECO { get; set; }
    }
}
