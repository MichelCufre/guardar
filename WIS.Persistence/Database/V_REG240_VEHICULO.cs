using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public class V_REG240_VEHICULO
    {
        [Key]
        public int CD_VEICULO { get; set; }
        public int? CD_TRANSPORTADORA { get; set; }
        [StringLength(15)]
        [Column]
        public string DS_PLACA { get; set; }
        [StringLength(100)]
        [Column]
        public string DS_VEICULO { get; set; }        
        public int CD_TIPO_VEICULO { get; set; }
        [StringLength(20)]
        [Column]
        public string DS_MARCA { get; set; }
        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
        public long HR_DISPONIBILIDAD_DESDE { get; set; }
        public long HR_DISPONIBILIDAD_HASTA { get; set; }
        [StringLength(20)]
        [Column]
        public string ND_ESTADO { get; set; }
        [StringLength(100)]
        [Column]
        public string DS_TRANSPORTADORA { get; set; }
        [StringLength(100)]
        [Column]
        public string DS_TIPO_VEICULO { get; set; }
        [StringLength(100)]
        [Column]
        public string DS_DOMINIO_VALOR { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_SYNC_REALIZADA { get; set; }
    }
}
