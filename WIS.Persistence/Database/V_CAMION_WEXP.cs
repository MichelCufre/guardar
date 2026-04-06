using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public class V_CAMION_WEXP
    {
        [Key]
        public int CD_CAMION { get; set; }
        public int? CD_EMPRESA { get; set; }
        [StringLength(15)]
        [Column]
        public string CD_PLACA_CARRO { get; set; }
        public short? CD_PORTA { get; set; }
        public short? CD_ROTA { get; set; }
        public short CD_SITUACAO { get; set; }
        public int CD_TRANSPORTADORA { get; set; }
        [StringLength(50)]
        [Column]
        public string DS_CAMION { get; set; }
        [StringLength(50)]
        [Column]
        public string DS_DOCUMENTO { get; set; }
        [StringLength(30)]
        [Column]
        public string DS_PORTA { get; set; }
        [StringLength(30)]
        [Column]
        public string DS_ROTA { get; set; }
        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }
        [StringLength(100)]
        [Column]
        public string DS_TRANSPORTADORA { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        [StringLength(1)]
        [Column]
        public string ID_RESPETA_ORD_CARGA { get; set; }
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }
        public long? NU_INTERFAZ_EJECUCION { get; set; }
        public long? NU_INTERFAZ_EJECUCION_FACT { get; set; }
        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
