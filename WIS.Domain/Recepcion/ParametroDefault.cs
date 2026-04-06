using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Recepcion
{
    public class ParametroDefault
    {
        public short NumeroParametro { get; set; } //NU_ALM_PARAMETRO
        public short? NumeroLogica { get; set; } //NU_ALM_LOGICA
        public string DescripcionParametro { get; set; } //DS_ALM_PARAMETRO
        public string Parametro { get; set; } //NM_ALM_PARAMETRO
        public string Valor { get; set; } //VL_ALM_PARAMETRO_DEFAULT
    }
}
