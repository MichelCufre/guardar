using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General.Enums;

namespace WIS.Domain.General
{
    public class ParametroConfiguracion
    {
        public int NuParametroConfiguracion { get; set; }   //NU_PARAMETRO
        public string CodigoParametro { get; set; }         //CD_PARAMETRO
        public string TipoParametro { get; set; }           //DO_ENTIDAD_PARAMETRIZABLE
        public string Clave { get; set; }                   //ND_ENTIDAD
        public string Valor { get; set; }                   //VL_PARAMETRO
    }
}
