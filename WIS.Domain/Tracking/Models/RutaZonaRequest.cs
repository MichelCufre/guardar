using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.Tracking.Models
{
    public class RutaZonaRequest
    {
        [ApiDtoExample("200")]
        [RequiredValidation]
        [StringLengthValidation(10, MinimumLength = 1)]
        public string Zona { get; set; }

        [ApiDtoExample("Zona")]
        [RequiredValidation]
        [StringLengthValidation(30, MinimumLength = 1)]
        public string Descripcion { get; set; }
    }
}
