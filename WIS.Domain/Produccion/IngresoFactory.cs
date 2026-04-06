using WIS.Domain.Produccion.Enums;
using WIS.Exceptions;

namespace WIS.Domain.Produccion
{
    public class IngresoFactory
    {
        public virtual IIngreso CreateIngreso(TipoProduccionIngreso tipo)
        {
            switch (tipo)
            {
                case TipoProduccionIngreso.BlackBox:
                    return new IngresoBlackBox();
                case TipoProduccionIngreso.Colector:
                    return new IngresoColector();
                case TipoProduccionIngreso.PanelWeb:
                    return new IngresoWhiteBox();
            }

            throw new ValidationFailedException("General_Sec0_Error_Error01");
        }

        public virtual IIngresoColector CreateIngresoColector(TipoProduccionIngreso tipo)
        {
            var ingreso = CreateIngreso(tipo);

            if (!(ingreso is IIngresoColector))
                throw new ValidationFailedException("General_Sec0_Error_Error01");

            return ingreso as IIngresoColector;
        }

        public virtual IIngresoPanel CreateIngresoPanel(TipoProduccionIngreso tipo)
        {
            var ingreso = CreateIngreso(tipo);

            if (!(ingreso is IIngresoPanel))
                throw new ValidationFailedException("General_Sec0_Error_Error01");

            return ingreso as IIngresoPanel;
        }
    }
}
