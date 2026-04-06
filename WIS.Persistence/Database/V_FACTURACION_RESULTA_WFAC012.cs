using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WIS.Persistence.Database
{
    public class V_FACTURACION_RESULTA_WFAC012
    {
        [Key]
        public int NU_EJECUCION { get; set; }
        
        [Key]
        public int CD_EMPRESA { get; set; }
        
        [Key]
        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }


        [StringLength(55)]
        public string NM_EMPRESA { get; set; }
        
        [StringLength(20)]
        public string CD_FACTURACION { get; set; }
        
        [StringLength(100)]
        public string DS_SIGNIFICADO { get; set; }

        [StringLength(10)]
        public string CD_UNIDADE_MEDIDA { get; set; }

        [StringLength(15)]
        public string CD_MONEDA { get; set; }

        [StringLength(80)]
        public string DS_MONEDA { get; set; }

        [StringLength(4)]
        public string DS_SIMBOLO { get; set; }

        [StringLength(30)]
        public string DS_SITUACAO { get; set; }

        [StringLength(10)]
        public string NU_CUENTA_CONTABLE { get; set; }

        [StringLength(100)]
        public string DS_CUENTA_CONTABLE { get; set; }
        
        public short? CD_SITUACAO { get; set; }
        public decimal? QT_RESULTADO { get; set; }
        public decimal? VL_PRECIO_UNITARIO { get; set; }
        public decimal? VL_PRECIO_MINIMO { get; set; }
        public decimal? IM_LINEA { get; set; }
        public DateTime? DT_APROBACION_RECHAZO { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
    }
}
