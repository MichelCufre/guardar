using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_DOCUMENTO_ANU_PREP_RESERVA")]
    public partial class T_DOCUMENTO_ANU_PREP_RESERVA
    {
        public int? NU_PREPARACION { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        public decimal? CD_FAIXA { get; set; }

        [StringLength(1)]
        public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_ANULAR { get; set; }

        [Key]
        [StringLength(256)]
        public string ID_ANULACION { get; set; }

        [StringLength(6)]
        public string ID_ESTADO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDATEROW { get; set; }
    }

}
