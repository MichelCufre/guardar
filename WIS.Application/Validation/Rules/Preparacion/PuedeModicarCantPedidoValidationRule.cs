using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class PuedeModicarCantPedidoValidationRule : IValidationRule
    {
        protected readonly decimal QtPedido;
        protected readonly decimal QtLiberado;
        protected readonly decimal QtAnulado;
        protected readonly decimal QtPedidoOriginal;

        public PuedeModicarCantPedidoValidationRule(string QT_PEDIDO, string QT_LIBERADO, string QT_ANULADO, string QT_PEDIDO_ORIGINAL, IFormatProvider culture)
        {
            decimal.TryParse(QT_PEDIDO, NumberStyles.Number, culture, out this.QtPedido);
            decimal.TryParse(QT_LIBERADO, NumberStyles.Number, culture, out this.QtLiberado);
            decimal.TryParse(QT_ANULADO, NumberStyles.Number, culture, out this.QtAnulado);
            decimal.TryParse(QT_PEDIDO_ORIGINAL, NumberStyles.Number, culture, out this.QtPedidoOriginal);

        }
        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            if (QtPedido > QtPedidoOriginal)
            {
                errors.Add(new ValidationError("PRE_Sec0_Error_NoPuedeModificarCantidadMayorOriginal"));
            }
            if (QtAnulado > 0 && QtLiberado > 0)
            {
                if (QtPedido < (QtLiberado + QtAnulado))
                {
                    errors.Add(new ValidationError("PRE_Sec0_Error_NoPuedeModificarCantidadMenorAnuladaLiberada"));
                }
            }
            else
            {
                if (QtLiberado > 0)
                {
                    if (QtPedido < QtLiberado)
                    {
                        errors.Add(new ValidationError("PRE_Sec0_Error_NoPuedeModificarCantidadMenorLiberado"));
                    }
                }
                if (QtAnulado > 0)
                {
                    if (QtPedido < QtAnulado)
                    {
                        errors.Add(new ValidationError("PRE_Sec0_Error_NoPuedeModificarCantidadMenorAnulada"));
                    }
                }

            }


            return errors;
        }
    }
}
