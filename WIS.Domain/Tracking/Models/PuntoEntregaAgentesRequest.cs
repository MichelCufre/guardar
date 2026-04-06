using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.Tracking.Models
{
    public class PuntoEntregaAgentesRequest
    {
        /// <summary>
        /// Código de punto de entrega
        /// </summary>
        [ApiDtoExample("1")]
        [RequiredValidation]
        [StringLengthValidation(20, MinimumLength = 1)]
        public string CodigoPuntoEntrega { get; set; }

        /// <summary>
        /// Zona donde se ubica el punto de entrega
        /// </summary>
        /// <example>S/G</example>
        [ApiDtoExample("200")]
        [RequiredValidation]
        [StringLengthValidation(10, MinimumLength = 1)]
        [DefaultValue("S/G")]
        public string Zona { get; set; }

        /// <summary>
        /// Lista de agentes que interactua con el punto de entrega
        /// </summary>
        [RequiredListValidation]
        public List<PuntoEntregaAgenteRequest> Agentes { get; set; }

        public PuntoEntregaAgentesRequest()
        {
            Agentes = new List<PuntoEntregaAgenteRequest>();
        }
    }

    public class PuntoEntregaAgenteRequest
    {
        /// <summary>
        /// Código de agente
        /// </summary>
        /// <example>1/001</example>
        [ApiDtoExample("CLI")]
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string Codigo { get; set; }

        /// <summary>
        /// Tipo de agente
        /// </summary>
        /// <example>CLI</example>
        [ApiDtoExample("CLI")]
        [RequiredValidation]
        [StringLengthValidation(3, MinimumLength = 1)]
        public string Tipo { get; set; }

        /// <summary>
        /// Código de empresa
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RequiredValidation]
        public int CodigoEmpresa { get; set; }

    }
}
