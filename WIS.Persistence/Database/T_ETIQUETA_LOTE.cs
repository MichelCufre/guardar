namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_ETIQUETA_LOTE")]
    public partial class T_ETIQUETA_LOTE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_ETIQUETA_LOTE()
        {
            T_DET_ETIQUETA_LOTE = new HashSet<T_DET_ETIQUETA_LOTE>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_ETIQUETA_LOTE { get; set; }

        public int NU_AGENDA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_SUGERIDO { get; set; }

        public short? CD_SITUACAO { get; set; }

        public int? CD_FUNC_RECEPCION { get; set; }

        public DateTime? DT_RECEPCION { get; set; }

        public int? CD_FUNC_ALMACENAMIENTO { get; set; }

        public DateTime? DT_ALMACENAMIENTO { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_GRUPO { get; set; }

        public short? CD_PALLET { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_BARRAS { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_MOVTO_PARCIAL { get; set; }

        public short? CD_SITUACAO_PALLET { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string NU_EXTERNO_ETIQUETA { get; set; }

        [Required]
        [StringLength(3)]
        [Column]
        public string TP_ETIQUETA { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public int? NU_UNIDAD_TRANSPORTE { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public long? NU_LPN { get; set; }

        public virtual T_AGENDA T_AGENDA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DET_ETIQUETA_LOTE> T_DET_ETIQUETA_LOTE { get; set; }

        public virtual T_ENDERECO_ESTOQUE T_ENDERECO_ESTOQUE { get; set; }

        public virtual T_ENDERECO_ESTOQUE T_ENDERECO_ESTOQUE_SUGERIDO { get; set; }
    }
}
