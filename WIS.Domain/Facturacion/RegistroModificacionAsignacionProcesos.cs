using NLog;
using WIS.Domain.DataModel;

namespace WIS.Domain.Facturacion
{
    public class RegistroModificacionAsignacionProcesos
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroModificacionAsignacionProcesos(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        /// <summary>
        /// + Crea Facturacion ejecucion empresa
        /// </summary>
        /// <param name="facturacionEjecucionEmpresa">Facturacion ejecucion empresa a crear</param>
        /// <returns>Retorna la facturacion con toda la informacion</returns>
        public virtual FacturacionEjecucionEmpresa RegistrarFacturacionEjecucionEmpresa(FacturacionEjecucionEmpresa facturacionEjecucionEmpresa)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.FacturacionRepository.AddFacturacionEjecucionEmpresa(facturacionEjecucionEmpresa);

                return facturacionEjecucionEmpresa;
            }
        }

        /// <summary>
        /// + Borra Facturacion ejecucion empresa
        /// </summary>
        /// <param name="nuEjecucion">Numero de ejecucion de facturacion a Borrar</param>
        /// <param name="cdEmpresa">Codigo de empresa de facturacion a Borrar</param>
        /// <param name="cdProceso">Codigo de proceso de facturacion a Borrar</param>
        public virtual void RemoverFacturacionEjecucionEmpresa(int nuEjecucion, int cdEmpresa, string cdProceso)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.FacturacionRepository.DeleteFacturacionEjecucionEmpresa(nuEjecucion, cdEmpresa, cdProceso);
            }
        }
    }
}
