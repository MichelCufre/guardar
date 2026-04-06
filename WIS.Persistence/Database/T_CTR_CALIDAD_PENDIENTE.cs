using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public class T_CTR_CALIDAD_PENDIENTE
    {
        [Key]
        public int NU_CTR_CALIDAD_PENDIENTE { get; set; }

        public int? CD_CONTROL { get; set; }

        [Column]
        public string CD_ENDERECO { get; set; }

        public int? NU_ETIQUETA { get; set; }

        public int? CD_EMPRESA { get; set; }

        [Column]
        public string CD_PRODUTO { get; set; }

        [Column]
        public string ID_ACEPTADO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public decimal? CD_FAIXA { get; set; }
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        public int? CD_FUNCIONARIO_ACEPTO { get; set; }

        [Column]
        public string NU_PREDIO { get; set; }

        public long? NU_LPN { get; set; }

        public int? ID_LPN_DET { get; set; }

        public string DS_CONTROL { get; set; }

        public long? NU_INSTANCIA_CONTROL { get; set; }
    }
}
