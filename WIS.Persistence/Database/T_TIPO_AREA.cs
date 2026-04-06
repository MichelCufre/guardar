namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_TIPO_AREA")]
    public partial class T_TIPO_AREA
    {
        public T_TIPO_AREA()
        {
            this.T_ENDERECO_ESTOQUE = new HashSet<T_ENDERECO_ESTOQUE>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CD_AREA_ARMAZ { get; set; }

        [Required]
        [StringLength(15)]
        [Column]
        public string DS_AREA_ARMAZ { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_ASSIST_TECNICA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AREA_PROBLEMA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AREA_PENDENCIA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AREA_EMBARQUE { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AREA_AVARIA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_RUA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_VEICULO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AREA_ESPERA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AREA_PICKING { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_ESTOQUE_GERAL { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_DISP_ESTOQUE { get; set; }

        public short? CD_AREA_ARMAZ_ANTERIOR { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_INVENTARIABLE { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_PERMITE_MANTENIMIENTO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_PERMITE_ALMACENAR { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_ENDERECO_ESTOQUE> T_ENDERECO_ESTOQUE { get; set; }
    }
}
