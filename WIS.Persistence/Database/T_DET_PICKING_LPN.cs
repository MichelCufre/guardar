using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_DET_PICKING_LPN")]
    public partial class T_DET_PICKING_LPN
    {
        [Key]
        [Column(Order = 0)]
        public int NU_PREPARACION { get; set; }
        [Key]
        [Column(Order = 1)]
        public long ID_DET_PICKING_LPN { get; set; }

        public int? ID_LPN_DET { get; set; }

        public long? NU_LPN { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        public decimal? CD_FAIXA { get; set; }

        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [StringLength(4000)]
        public string VL_ATRIBUTOS { get; set; }

        public decimal QT_RESERVA { get; set; }

        [StringLength(10)]
        public string TP_LPN_TIPO { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public string ID_LPN_EXTERNO { get; set; }

        public long? NU_DET_PED_SAI_ATRIB { get; set; }


    }
}
