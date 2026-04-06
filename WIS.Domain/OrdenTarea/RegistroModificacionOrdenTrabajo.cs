using NLog;
using WIS.Domain.DataModel;

namespace WIS.Domain.OrdenTarea
{
    public class RegistroModificacionOrdenTrabajo
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroModificacionOrdenTrabajo(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        /// <summary>
        /// + Crea Orden
        /// </summary>
        /// <param name="orden">Orden a crear</param>
        /// <returns>Retorna la orden con toda la informacion</returns>
        public virtual Orden RegistrarOrden(Orden orden)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.OrdenRepository.AddOrden(orden);

                return orden;
            }
        }

        /// <summary>
        /// + Actualiza la orden ingresada
        /// </summary>
        /// <param name="orden">Orden a actualizar</param>
        /// <returns>Retorna la orden modificada</returns>
        public virtual Orden ModificarOrden(Orden orden)
        {
            this._uow.OrdenRepository.UpdateOrden(orden);

            return orden;
        }
    }
}
