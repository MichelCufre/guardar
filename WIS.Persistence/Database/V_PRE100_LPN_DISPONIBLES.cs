using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    public class V_PRE100_LPN_DISPONIBLES
    {
        [Key]
        [Column(Order = 0)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 1)]
        public string CD_CLIENTE { get; set; }

        [Key]
        [Column(Order = 2)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(50)]
        public string ID_LPN_EXTERNO { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(10)]
        public string TP_LPN_TIPO { get; set; }

        public long NU_LPN { get; set; }

        [StringLength(400)]
        public string DS_LPN_TIPO { get; set; }

        [StringLength(6)]
        public string ID_ESTADO { get; set; }

        [StringLength(100)]
        public string DS_ESTADO { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [StringLength(50)]
        public string ID_PACKING { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }

    }
}
