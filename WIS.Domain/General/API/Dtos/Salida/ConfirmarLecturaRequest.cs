using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Salida
{
    public class ConfirmarLecturaRequest
    {
        /// <summary>
        /// Código de empresa de la ejecución
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RequiredValidation]
        [ExisteEmpresaValidation()]
        public int Empresa { get; set; }

        /// <summary>
        /// Número de interfaz de ejecución
        /// </summary>
        /// <example>6454</example>
        [ApiDtoExample("6454")]
        [RequiredValidation]
        [InterfazEjecucionValidation()]
        public long NumeroInterfazEjecucion { get; set; }

        /// <summary>
        /// Código de la interfaz externa de de la ejecución
        /// Ejemplo 502 (Confirmación de recepción)
        /// </summary>
        /// <example>502</example>
        [ApiDtoExample("502")]
        [RequiredValidation]
        [InterfazExternaValidation]
        public int CodigoInterfazExterna { get; set; }

        /// <summary>
        /// Estado final de la interfaz
        /// </summary>
        /// <example>true o false</example>
        [ApiDtoExample("true o false")]
        [RequiredValidation]
        public bool Resultado { get; set; }

        /// <summary>
        /// Lista de errores de la interfaz.
        /// </summary>
        public List<string> Errores { get; set; }
        public ConfirmarLecturaRequest()
        {
            Errores = new List<string>();
        }
    }
}
