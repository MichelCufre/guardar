namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_FACTURACION_PALLET")]
    public partial class T_FACTURACION_PALLET
    {
        public T_FACTURACION_PALLET()
        {
            this.T_FACTURACION_PALLET_DET = new HashSet<T_FACTURACION_PALLET_DET>();
        }

        [Key]
        [StringLength(40)]
        public string NU_PALLET { get; set; }
        
        [StringLength(1)]
        public string FL_DEST_SIN_PRODUCTO { get; set; }
        
        public int? NU_ETIQUETA_LOTE { get; set; }

        public int CD_EMPRESA { get; set; }

        public int? NU_AGENDA_INGRESO { get; set; }

        public decimal? QT_UND_INGRESO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_DESTRUCCION_PALLET { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public virtual ICollection<T_FACTURACION_PALLET_DET> T_FACTURACION_PALLET_DET { get; set; }
    }
}
