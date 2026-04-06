using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_INV411_UBIC_DISP")]
    public partial class V_INV411_UBIC_DISP
    {
        [Key]
        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [StringLength(10)]
        public string ID_BLOQUE { get; set; }

        public int? NU_COLUMNA { get; set; }

        public int? NU_ALTURA { get; set; }

        public short CD_TIPO_ENDERECO { get; set; }

        public short CD_ROTATIVIDADE { get; set; }

        public int CD_FAMILIA_PRINCIPAL { get; set; }
        
        [StringLength(2)]
        public string CD_CLASSE { get; set; }

        public short CD_SITUACAO { get; set; }

        [StringLength(30)]
        public string DS_SITUACAO { get; set; }

        [StringLength(1)]
        public string ID_ENDERECO_BAIXO { get; set; }

        [StringLength(1)]
        public string ID_ENDERECO_SEP { get; set; }

        [StringLength(1)]
        public string ID_NECESSIDADE_RESUPRIR { get; set; }

        [StringLength(5)]
        public string CD_CONTROL { get; set; }

        public short? CD_AREA_ARMAZ { get; set; }

        [StringLength(15)]
        public string DS_AREA_ARMAZ { get; set; }

        [StringLength(4)]
        public string NU_COMPONENTE { get; set; }

        [StringLength(10)]
        public string ID_CALLE { get; set; }

        public decimal NU_INVENTARIO { get; set; }

        public int? NU_PROFUNDIDAD { get; set; }
    }
}
