using NLog;
using WIS.Domain.DataModel;

namespace WIS.Domain.Facturacion
{
    public class RegistroProcesoFacturacion
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroProcesoFacturacion(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }


        public virtual FacturacionProceso RegistrarProcesoFacturacion(FacturacionProceso facturacionProceso)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.FacturacionRepository.AddFacturacionProceso(facturacionProceso);

                return facturacionProceso;
            }
        }

        public virtual FacturacionProceso UpdateProcesoFacturacion(FacturacionProceso facturacionProceso)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.FacturacionRepository.UpdateFacturacionProceso(facturacionProceso);

                return facturacionProceso;
            }
        }
    }
}
