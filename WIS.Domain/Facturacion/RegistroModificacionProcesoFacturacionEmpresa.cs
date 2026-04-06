using NLog;
using WIS.Domain.DataModel;

namespace WIS.Domain.Facturacion
{
    public class RegistroModificacionProcesoFacturacionEmpresa
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroModificacionProcesoFacturacionEmpresa(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        /// <summary>
        /// + Crea facturacion empresa proceso
        /// </summary>
        /// <param name="facturacionEmpresaProceso">Facturacion empresa proceso a crear</param>
        /// <returns>Retorna la facturacion con toda la informacion</returns>
        public virtual FacturacionEmpresaProceso RegistrarProcesoFacturacionEmpresa(FacturacionEmpresaProceso facturacionEmpresaProceso)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.FacturacionRepository.AddFacturacionEmpresaProceso(facturacionEmpresaProceso);

                return facturacionEmpresaProceso;
            }
        }

        /// <summary>
        /// + Actualiza la facturacion empresa proceso ingresada
        /// </summary>
        /// <param name="facturacionEmpresaProceso">Facturacion empresa proceso a actualizar</param>
        /// <returns>Retorna la facturacion modificada</returns>
        public virtual FacturacionEmpresaProceso ModificarProcesoFacturacionEmpresa(FacturacionEmpresaProceso facturacionEmpresaProceso)
        {
            this._uow.FacturacionRepository.UpdateFacturacionEmpresaProceso(facturacionEmpresaProceso);

            return facturacionEmpresaProceso;
        }
    }
}
