using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class ProduccionProductoFinalesRequest
    {
        [ApiDtoExample("PR1")]
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string CodigoProducto { get; set; }

        [ApiDtoExample("100")]
        [RequiredValidation]
        [RangeValidation(12, 3)]
        public decimal CantidadTeorica { get; set; }
        
        [ApiDtoExample("Anexo1")]
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo1 { get; set; }

        [ApiDtoExample("Anexo2")]
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo2 { get; set; }

        [ApiDtoExample("Anexo3")]
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo3 { get; set; }

        [ApiDtoExample("Anexo4")]
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo4 { get; set; }

    }
}
