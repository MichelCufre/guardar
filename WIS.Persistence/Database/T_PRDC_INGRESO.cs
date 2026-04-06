using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WIS.Persistence.Database
{
    public partial class T_PRDC_INGRESO
    {
        public T_PRDC_INGRESO()
        {
            this.T_PRDC_INGRESO_DOCUMENTO = new HashSet<T_PRDC_INGRESO_DOCUMENTO>();
            this.T_PRDC_INGRESO_PASADA = new HashSet<T_PRDC_INGRESO_PASADA>();
        }

        [Key]
        [Column]
        public string NU_PRDC_INGRESO { get; set; }

        [Column]
        public string CD_PRDC_DEFINICION { get; set; }

        public int? QT_FORMULA { get; set; }

        public int? CD_EMPRESA { get; set; }

        [Column]
        public string ID_GENERAR_PEDIDO { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        public short? CD_SITUACAO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [Column]
        public string NU_PRDC_ORIGINAL { get; set; }

        [Column]
        public string DS_ANEXO1 { get; set; }

        [Column]
        public string DS_ANEXO2 { get; set; }

        [Column]
        public string DS_ANEXO3 { get; set; }

        [Column]
        public string DS_ANEXO4 { get; set; }

        [Column]
        public string DS_ANEXO5 { get; set; }

        [Column]
        public string ID_PRODUCCION_EXTERNO { get; set; }

        [Column]
        public string ND_ASIGNACION_LOTE { get; set; }

        [Column]
        public string NU_LOTE { get; set; }

        [Column]
        public string CD_PRODUTO_INSUMO_ANCLA_LOTE { get; set; }

        [Column]
        public int? NU_POSICION_EN_COLA { get; set; }

        [Column]
        public string FL_INGRESO_DIRECTO_A_PRODUCCION { get; set; }

        [Column]
        public string FL_PERMITIR_AUTOASIGNAR_LINEA { get; set; }

        public long? NU_INTERFAZ_EJECUCION_ENTRADA { get; set; }

        [Column]
        public string ND_TIPO { get; set; }

        [Column]
        public string CD_PRDC_LINEA { get; set; }

        [Column]
        public string MODALIDAD_TRABAJO { get; set; }

        [Column]
        public string NU_PREDIO { get; set; }

        [Column]
        public long? NU_TRANSACCION { get; set; }

        public DateTime? DT_INICIO_PRODUCCION { get; set; }

        public DateTime? DT_FIN_PRODUCCION { get; set; }

        [Column]
        public string ID_MANUAL { get; set; }

        public long? NU_ULT_INTERFAZ_EJECUCION { get; set; }

        [Column]
        public string TP_FLUJO { get; set; }

        public virtual T_PRDC_DEFINICION T_PRDC_DEFINICION { get; set; }
        public virtual T_SITUACAO T_SITUACAO { get; set; }
        public virtual ICollection<T_PRDC_INGRESO_DOCUMENTO> T_PRDC_INGRESO_DOCUMENTO { get; set; }
        public virtual ICollection<T_PRDC_INGRESO_PASADA> T_PRDC_INGRESO_PASADA { get; set; }

	}
}
