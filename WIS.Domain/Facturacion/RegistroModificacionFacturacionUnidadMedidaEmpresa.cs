using NLog;
using WIS.Domain.DataModel;

namespace WIS.Domain.Facturacion
{
    public class RegistroModificacionFacturacionUnidadMedidaEmpresa
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroModificacionFacturacionUnidadMedidaEmpresa(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        /// <summary>
        /// + Crea Facturacion unidad medida empresa
        /// </summary>
        /// <param name="facturacionUnidadMedidaEmpresa">Facturacion unidad medida empresa a crear</param>
        /// <returns>Retorna la facturacion con toda la informacion</returns>
        public virtual FacturacionUnidadMedidaEmpresa RegistrarFacturacionUnidadMedidaEmpresa(FacturacionUnidadMedidaEmpresa facturacionUnidadMedidaEmpresa)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.FacturacionRepository.AddFacturacionUnidadMedidaEmpresa(facturacionUnidadMedidaEmpresa);

                return facturacionUnidadMedidaEmpresa;
            }
        }
    }
}
