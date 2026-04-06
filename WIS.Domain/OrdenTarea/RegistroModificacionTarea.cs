using NLog;
using WIS.Domain.DataModel;

namespace WIS.Domain.OrdenTarea
{
    public class RegistroModificacionTarea
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroModificacionTarea(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        /// <summary>
        /// + Crea Tarea
        /// </summary>
        /// <param name="tarea">Tarea a crear</param>
        /// <returns>Retorna la tarea con toda la informacion</returns>
        public virtual Tarea RegistrarTarea(Tarea tarea)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.TareaRepository.AddTarea(tarea);

                return tarea;
            }
        }

        /// <summary>
        /// + Actualiza la tarea ingresada
        /// </summary>
        /// <param name="tarea">Tarea a actualizar</param>
        /// <returns>Retorna la tarea modificada</returns>
        public virtual Tarea ModificarTarea(Tarea tarea)
        {
            this._uow.TareaRepository.UpdateTarea(tarea);

            return tarea;
        }
    }
}
