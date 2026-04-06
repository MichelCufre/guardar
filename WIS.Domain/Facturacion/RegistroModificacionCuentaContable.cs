using NLog;
using WIS.Domain.DataModel;

namespace WIS.Domain.Facturacion
{
    public class RegistroModificacionCuentaContable
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroModificacionCuentaContable(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        /// <summary>
        /// + Crea cuenta contable
        /// </summary>
        /// <param name="cuentaContable">Cuenta contable a crear</param>
        /// <returns>Retorna la cuenta con toda la informacion</returns>
        public virtual CuentaContable RegistrarLenguajeImpresion(CuentaContable cuentaContable)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.CuentaContableRepository.AddCuentaContable(cuentaContable);

                return cuentaContable;
            }
        }

        /// <summary>
        /// + Actualiza la cuenta contable ingresada
        /// </summary>
        /// <param name="cuentaContable">Cuenta contablea a actualizar</param>
        /// <returns>Retorna la cuenta contable modificada</returns>
        public virtual CuentaContable ModificarLenguajeImpresion(CuentaContable cuentaContable)
        {
            this._uow.CuentaContableRepository.UpdateCuentaContable(cuentaContable);

            return cuentaContable;
        }
    }
}
