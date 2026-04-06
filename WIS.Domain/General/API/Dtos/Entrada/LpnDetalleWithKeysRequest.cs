using System;
using System.Collections.Generic;
using System.Text;
using WIS.WMS_API.Controllers.Entrada;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class LpnDetalleWithKeysRequest : LpnDetalleRequest
    {

        /// <summary>
        /// Identificador del ERP o sistema externo para el Lpn.
        /// </summary>
        /// <example>AZ-39</example>
        public string IdExterno { get; set; }

        public LpnDetalleWithKeysRequest() : base()
        {

        }
    }
}
