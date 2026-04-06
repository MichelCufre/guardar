using System;
using System.Collections.Generic;
using System.Text;
using WIS.WMS_API.Controllers.Entrada;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class LpnAtributoDetalleWithKeysRequest : AtributoRequest
    {
        /// <summary>
        /// Identificador del ERP o sistema externo para el Lpn.
        /// </summary>
        /// <example>AZ-39</example>
        public string IdExterno { get; set; }

        /// <summary>
        /// Identificador en el ERP o sistema externo para la línea del detalle. 
        /// </summary>
        /// <example>1</example>
        public string IdLineaSistemaExterno { get; set; }

        public LpnAtributoDetalleWithKeysRequest() : base()
        {

        }
    }
}
