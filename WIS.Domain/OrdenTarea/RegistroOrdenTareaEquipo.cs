using NLog;
using WIS.Domain.DataModel;

namespace WIS.Domain.OrdenTarea
{
    public class RegistroOrdenTareaEquipo
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroOrdenTareaEquipo(IUnitOfWork uow, int _userId, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = _userId;
            this._aplicacion = aplicacion;
        }


        public virtual OrdenTareaEquipo RegistrarOrdenTareaEquipo(OrdenTareaEquipo ordenEquipo)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.TareaRepository.AddOrdenTareaEquipo(ordenEquipo);

                return ordenEquipo;
            }
        }
        public virtual OrdenTareaEquipo UpdateOrdenTareaEquipo(OrdenTareaEquipo ordenEquipo)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.TareaRepository.UpdateOrdenTareaEquipo(ordenEquipo);

                return ordenEquipo;
            }
        }




    }
}
