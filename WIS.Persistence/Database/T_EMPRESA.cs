namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_EMPRESA")]
    public partial class T_EMPRESA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_EMPRESA()
        {
            T_PRODUTO = new HashSet<T_PRODUTO>();
            T_ENDERECO_ESTOQUE = new HashSet<T_ENDERECO_ESTOQUE>();
            T_RECEPCION_REL_EMPRESA_TIPO = new HashSet<T_RECEPCION_REL_EMPRESA_TIPO>();
            T_RECEPCION_REFERENCIA = new HashSet<T_RECEPCION_REFERENCIA>();
            T_PRDC_DEFINICION = new HashSet<T_PRDC_DEFINICION>();
            T_EMPRESA_FUNCIONARIO = new HashSet<T_EMPRESA_FUNCIONARIO>();
            T_ALM_SUGERENCIA = new HashSet<T_ALM_SUGERENCIA>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        public short CD_SITUACAO { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public DateTime DT_ADDROW { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ENDERECO { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_TELEFONE { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_CGC_EMPRESA { get; set; }

        [StringLength(1)]
        [Column]
        public string FG_QUEBRA_PEDIDO { get; set; }

        public int? CD_FORN_DEVOLUCAO { get; set; }

        public decimal? VL_POS_PALETE { get; set; }

        public decimal? VL_POS_PALETE_DIA { get; set; }

        public short? QT_DIAS_POR_PERIODO { get; set; }

        [StringLength(15)]
        [Column]
        public string DS_CP_POSTAL { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE_ARMADO_KIT { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO3 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO4 { get; set; }

        public decimal? IM_MINIMO_STOCK { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_UND_FACT_EMPRESA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_OPERATIVO { get; set; }

        public short? TP_ALMACENAJE_Y_SEGURO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_DAP { get; set; }

        public int? CD_EMPRESA_DE_CONSOLIDADO { get; set; }

        public long? ID_LOCALIDAD { get; set; }
        public int? CD_LISTA_PRECIO { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_TIPO_FISCAL { get; set; }

        [StringLength(20)]
        [Column]
        public string TP_NOTIFICACION { get; set; }

        [StringLength(2048)]
        [Column]
        public string VL_PAYLOAD_URL { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_LOCKED { get; set; }

        [StringLength(4000)]
        [Column]
        public string VL_SECRET { get; set; }

        [StringLength(4000)]
        [Column]
        public string VL_SECRETSALT { get; set; }

        public decimal? VL_SECRETFORMAT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PRODUTO> T_PRODUTO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_EMPRESA_FUNCIONARIO> T_EMPRESA_FUNCIONARIO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_ENDERECO_ESTOQUE> T_ENDERECO_ESTOQUE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_RECEPCION_REL_EMPRESA_TIPO> T_RECEPCION_REL_EMPRESA_TIPO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_RECEPCION_REFERENCIA> T_RECEPCION_REFERENCIA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PRDC_DEFINICION> T_PRDC_DEFINICION { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_ALM_SUGERENCIA> T_ALM_SUGERENCIA { get; set; }
    }
}
