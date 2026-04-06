using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    public class V_CTR_CALIDAD_WSTO110
    {
        [Key]
        public int NU_CTR_CALIDAD_PENDIENTE { get; set; }
        public int? CD_CONTROL { get; set; }
        [Column]
        public string DS_CONTROL { get; set; }
        [Column]
        public string CD_ENDERECO { get; set; }
        public int? NU_ETIQUETA { get; set; }
        public int? CD_EMPRESA { get; set; }
        [Column]
        public string CD_PRODUTO { get; set; }
        [Column]
        public string DS_PRODUTO { get; set; }
        [Column]
        public string ID_ACEPTADO { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        public decimal? CD_FAIXA { get; set; }
        [Column]
        public string NU_IDENTIFICADOR { get; set; }
        public int? CD_FUNCIONARIO_ACEPTO { get; set; }
        [Column]
        public string NM_FUNCIONARIO { get; set; }
        [Column]
        public string NU_EXTERNO_ETIQUETA { get; set; }
        [Column]
        public string TP_ETIQUETA { get; set; }
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
