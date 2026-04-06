using WIS.Domain.Facturacion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class CuentaContablenMapper
    {
        public CuentaContablenMapper()
        {

        }

        public virtual CuentaContable MapToObject(T_FACTURACION_CUENTA_CONTABLE entity)
        {
            return new CuentaContable
            {
                Id = entity.NU_CUENTA_CONTABLE,
                Descripcion = entity.DS_CUENTA_CONTABLE
            };
        }
        public virtual T_FACTURACION_CUENTA_CONTABLE MapToEntity(CuentaContable cuentaContable)
        {
            return new T_FACTURACION_CUENTA_CONTABLE
            {
                NU_CUENTA_CONTABLE = cuentaContable.Id,
                DS_CUENTA_CONTABLE = cuentaContable.Descripcion
            };
        }
    }
}
