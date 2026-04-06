namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_TRANSPORTADORA")]
    public partial class T_TRANSPORTADORA
    {
        public T_TRANSPORTADORA()
        {
            T_DOCUMENTO_AGRUPADOR = new HashSet<T_DOCUMENTO_AGRUPADOR>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_TRANSPORTADORA { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TRANSPORTADORA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ENDERECO { get; set; }

        [StringLength(5)]
        [Column]
        public string CD_UF { get; set; }

        public int? CD_MUNICIPIO { get; set; }

        public DateTime? DT_SITUACAO { get; set; }

        public DateTime? DT_CADASTRAMENTO { get; set; }

        public DateTime? DT_ALTERACAO { get; set; }

        public long? CD_CGC_TRANSP { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_INSCRICAO_TRANSP { get; set; }

        [StringLength(60)]
        [Column]
        public string NM_CONTACTO { get; set; }

        [StringLength(20)]
        [Column]
        public string NU_FAX { get; set; }

        [StringLength(20)]
        [Column]
        public string NU_TELEFONE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DOCUMENTO_AGRUPADOR> T_DOCUMENTO_AGRUPADOR { get; set; }
    }
}
