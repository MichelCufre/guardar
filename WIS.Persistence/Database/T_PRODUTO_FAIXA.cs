namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PRODUTO_FAIXA")]
    public partial class T_PRODUTO_FAIXA
    {
        public T_PRODUTO_FAIXA() 
        {
            T_PRDC_DET_SALIDA = new HashSet<T_PRDC_DET_SALIDA>();
            T_STOCK = new HashSet<T_STOCK>();
        }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 2)]
        public decimal CD_FAIXA { get; set; }

        public DateTime DT_UPDROW { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public int? QT_ESTOQUE { get; set; }

        public int? QT_RESERVA { get; set; }

        public decimal? QT_UNIDADE_EMBALAGEM { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_EMBALAGEM_FAIXA { get; set; }

        public decimal? VL_ALTURA { get; set; }

        public decimal? VL_LARGURA { get; set; }

        public decimal? VL_PROFUNDIDADE { get; set; }

        public virtual ICollection<T_PRDC_DET_SALIDA> T_PRDC_DET_SALIDA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_STOCK> T_STOCK { get; set; }

        public virtual T_PRODUTO T_PRODUTO { get; set; }
    }
}
