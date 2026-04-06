using NLog;
using WIS.Domain.DataModel;

namespace WIS.Domain.Facturacion
{
    public class RegistroModificacionIngresoResultadosManuales
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroModificacionIngresoResultadosManuales(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        /// <summary>
        /// + Crea facturacion resultado
        /// </summary>
        /// <param name="facturacionResultado">Facturacion resultado a crear</param>
        /// <returns>Retorna la facturacion con toda la informacion</returns>
        public virtual FacturacionResultado RegistrarFacturacionResultado(FacturacionResultado facturacionResultado)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.FacturacionRepository.AddFacturacionResultado(facturacionResultado);

                return facturacionResultado;
            }
        }

        /// <summary>
        /// + Actualiza la facturacion resultado ingresado
        /// </summary>
        /// <param name="facturacionResultado">Facturacion resultado a actualizar</param>
        /// <returns>Retorna la facturacion modificada</returns>
        public virtual FacturacionResultado ModificarFacturacionResultado(FacturacionResultado facturacionResultado)
        {
            this._uow.FacturacionRepository.UpdateFacturacionResultado(facturacionResultado);

            return facturacionResultado;
        }
    }
}
