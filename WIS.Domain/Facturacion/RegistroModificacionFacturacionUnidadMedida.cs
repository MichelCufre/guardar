using NLog;
using WIS.Domain.DataModel;

namespace WIS.Domain.Facturacion
{
    public class RegistroModificacionFacturacionUnidadMedida
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroModificacionFacturacionUnidadMedida(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        /// <summary>
        /// + Crea Facturacion unidad medida
        /// </summary>
        /// <param name="facturacionUnidadMedida">Facturacion unidad medida a crear</param>
        /// <returns>Retorna la facturacion con toda la informacion</returns>
        public virtual FacturacionUnidadMedida RegistrarFacturacionUnidadMedida(FacturacionUnidadMedida facturacionUnidadMedida)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.FacturacionRepository.AddFacturacionUnidadMedida(facturacionUnidadMedida);

                return facturacionUnidadMedida;
            }
        }

        /// <summary>
        /// + Actualiza Facturacion unidad medida ingresada
        /// </summary>
        /// <param name="facturacionUnidadMedida">Facturacion unidad medida a actualizar</param>
        /// <returns>Retorna la facturacion modificada</returns>
        public virtual FacturacionUnidadMedida ModificarFacturacionUnidadMedida(FacturacionUnidadMedida facturacionUnidadMedida)
        {
            this._uow.FacturacionRepository.UpdateFacturacionUnidadMedida(facturacionUnidadMedida);

            return facturacionUnidadMedida;
        }

    }
}
