using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Recepcion
{
    public class EmpresaRecepcionTipoReporte
    {
        public int Id { get; set; }                 //NU_REC_EMP_TIPO_REP
        public string TipoRecepcion { get; set; }   //TP_RECEPCION_EXTERNO
        public int? IdEmpresa { get; set; }         //CD_EMPRESA
        public string CodigoReporte { get; set; }   //CD_REPORTE
    }
}
