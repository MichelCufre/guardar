using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("T_DET_PALLET_TRANSFERENCIA")]
    public partial class T_DET_PALLET_TRANSFERENCIA
    {

        [Key]
        [Column(Order = 0)]
        public decimal NU_ETIQUETA { get; set; }

        [Key]
        [Column(Order = 1)]
        public int NU_SEC_ETIQUETA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string CD_ENDERECO_ORIGEN { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 5)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 6)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 7)]
        public int NU_SEC_DETALLE { get; set; }

        public short? CD_SITUACAO { get; set; }

        public decimal? QT_PRODUTO { get; set; }

        public DateTime? DT_FABRICACAO { get; set; }

        [StringLength(1)]
        public string ID_AVERIA { get; set; }

        [StringLength(1)]
        public string ID_INVENTARIO_CICLICO { get; set; }

        public DateTime? DT_ULT_INVENTARIO { get; set; }

        [StringLength(1)]
        public string ID_CTRL_CALI_PEND { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO_DESTINO { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(1)]
        public string ID_AREA_AVERIA { get; set; }

        public long? NU_TRANSACCION { get; set; }

        [StringLength(200)]
        public string VL_METADATA { get; set; }

        public long? NU_LPN { get; set; }

    }
}