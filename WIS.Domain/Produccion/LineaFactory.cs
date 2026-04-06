using WIS.Domain.Produccion.Enums;
using WIS.Exceptions;

namespace WIS.Domain.Produccion
{
    public class LineaFactory
    {
        public virtual ILinea Create(TipoProduccionLinea tipo)
        {
            switch (tipo)
            {
                case TipoProduccionLinea.WhiteBox:
                    return new LineaWhiteBox();
                case TipoProduccionLinea.BlackBox:
                    return new LineaBlackBox();
            }

            throw new ValidationFailedException("General_Sec0_Error_LineaNoRegistrada");
        }
    }
}
