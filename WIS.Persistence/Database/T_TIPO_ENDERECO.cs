using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_TIPO_ENDERECO")]
    public partial class T_TIPO_ENDERECO
    {
        public T_TIPO_ENDERECO()
        {
            this.T_TIPO_ENDERECO_PALLET = new HashSet<T_TIPO_ENDERECO_PALLET>();
            this.T_ENDERECO_ESTOQUE = new HashSet<T_ENDERECO_ESTOQUE>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CD_TIPO_ENDERECO { get; set; }

        [StringLength(30)]        
        public string DS_TIPO_ENDERECO { get; set; }

        public int? QT_CAPAC_PALETES { get; set; }

        [StringLength(1)]
        public string ID_VARIOS_PRODUTOS { get; set; }

        public short CD_TIPO_ESTRUTURA { get; set; }

        public short? CD_AREA_ARMAZ { get; set; }

        public decimal? VL_ALTURA { get; set; }

        public decimal? VL_LARGURA { get; set; }

        public decimal? VL_COMPRIMENTO { get; set; }

        public decimal? VL_PESO_MAXIMO { get; set; }

        public DateTime DT_UPDROW { get; set; }

        public DateTime DT_ADDROW { get; set; }

        [StringLength(1)]
        public string ID_VARIOS_LOTES { get; set; }

        public decimal? QT_VOLUMEN_UNIDAD_FACTURACION { get; set; }

        [StringLength(1)]
        public string FL_RESPETA_CLASE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_TIPO_ENDERECO_PALLET> T_TIPO_ENDERECO_PALLET { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_ENDERECO_ESTOQUE> T_ENDERECO_ESTOQUE { get; set; }
    }
}
