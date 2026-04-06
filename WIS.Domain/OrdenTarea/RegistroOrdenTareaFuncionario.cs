using NLog;
using WIS.Domain.DataModel;

namespace WIS.Domain.OrdenTarea
{
    public class RegistroOrdenTareaFuncionario
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroOrdenTareaFuncionario(IUnitOfWork uow, int _userId, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = _userId;
            this._aplicacion = aplicacion;
        }

       
        public virtual OrdenTareaFuncionario RegistrarOrdenTareaFuncionario(OrdenTareaFuncionario ordenTareaFuncionario)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.TareaRepository.AddOrdenTareaFuncionario(ordenTareaFuncionario);

                return ordenTareaFuncionario;
            }
        }
        public virtual OrdenTareaFuncionario UpdateOrdenTareaFuncionario(OrdenTareaFuncionario ordenTareaFuncionario)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.TareaRepository.UpdateOrdenTareaFuncionario(ordenTareaFuncionario);

                return ordenTareaFuncionario;
            }
        }
        



    }
}
