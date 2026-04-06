using NLog;
using WIS.Domain.DataModel;

namespace WIS.Domain.OrdenTarea
{
    public class RegistroOrdenTareaManipuleoInsumo
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroOrdenTareaManipuleoInsumo(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        public virtual OrdenTareaManipuleoInsumo RegistrarOrdenTareaManipuleoInsumo(OrdenTareaManipuleoInsumo ordenManipueleoInsumo)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.TareaRepository.AddOrdenManipuleoInsumo(ordenManipueleoInsumo);

                return ordenManipueleoInsumo;
            }
        }

        public virtual OrdenTareaManipuleoInsumo UpdateOrdenTareaManipuleoInsumo(OrdenTareaManipuleoInsumo ordenManipueleoInsumo)
        {
            this._uow.TareaRepository.UpdateOrdenTareaManipuleoInsumo(ordenManipueleoInsumo);

            return ordenManipueleoInsumo;
        }
    }
}
