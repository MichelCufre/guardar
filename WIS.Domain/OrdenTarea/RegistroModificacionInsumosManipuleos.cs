using NLog;
using WIS.Domain.DataModel;

namespace WIS.Domain.OrdenTarea
{
    public class RegistroModificacionInsumosManipuleos
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroModificacionInsumosManipuleos(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        /// <summary>
        /// + Crea insumo o manipuleo
        /// </summary>
        /// <param name="insumoManipuleo">Insumo o Manipuleo a crear</param>
        /// <returns>Retorna el insumo o manipuleo con toda la informacion</returns>
        public virtual InsumoManipuleo RegistrarInsumoManipuleo(InsumoManipuleo insumoManipuleo)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.InsumoManipuleoRepository.AddInsumoManipuleo(insumoManipuleo);

                return insumoManipuleo;
            }
        }

        /// <summary>
        /// + Actualiza el insumo o manipuleo ingresado
        /// </summary>
        /// <param name="insumoManipuleo">Tarea a actualizar</param>
        /// <returns>Retorna la tarea modificada</returns>
        public virtual InsumoManipuleo ModificarInsumoManipuleo(InsumoManipuleo insumoManipuleo)
        {
            this._uow.InsumoManipuleoRepository.UpdateInsumoManipuleo(insumoManipuleo);

            return insumoManipuleo;
        }
    }
}
