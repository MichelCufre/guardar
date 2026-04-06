using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_INVENTARIO_ENDERECO_DET")]
    public partial class T_INVENTARIO_ENDERECO_DET
    {
        [Key]
        public decimal NU_INVENTARIO_ENDERECO_DET { get; set; }

        public decimal NU_INVENTARIO_ENDERECO { get; set; }

        public decimal? NU_CONTEO { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_INVENTARIO { get; set; }

        public decimal? QT_DIFERENCIA { get; set; }

        public DateTime? DT_VENCIMIENTO { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_ESTADO_INV_ENDERECO_DET { get; set; }

        public int? CD_USUARIO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public decimal? QT_TIEMPO_INSUMIDO { get; set; }

        [StringLength(3)]
        [Column]
        public string CD_MOTIVO_AJUSTE { get; set; }

        public long? NU_INSTANCIA_CONTEO { get; set; }

        public decimal? CD_FAIXA { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public long? NU_LPN { get; set; }

        public int? ID_LPN_DET { get; set; }

        [StringLength(1)]
        public string FL_CONTEO_ESPERADO { get; set; }


        public virtual T_INVENTARIO_ENDERECO T_INVENTARIO_ENDERECO { get; set; }
    }
}
