using NLog;
using WIS.Domain.DataModel;

namespace WIS.Domain.Facturacion
{
    public class RegistroModificacionCotizacionListas
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroModificacionCotizacionListas(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        /// <summary>
        /// + Crea Cotizacion Listas
        /// </summary>
        /// <param name="cotizacionListas">Cotizacion Listas a crear</param>
        /// <returns>Retorna la cotizacion con toda la informacion</returns>
        public virtual CotizacionListas RegistrarCotizacionListas(CotizacionListas cotizacionListas)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.CotizacionListasRepository.AddCotizacionListas(cotizacionListas);

                return cotizacionListas;
            }
        }

        /// <summary>
        /// + Actualiza la cotizacion ingresada
        /// </summary>
        /// <param name="cotizacionListas">Cotizacion a actualizar</param>
        /// <returns>Retorna la cotizacion modificada</returns>
        public virtual CotizacionListas ModificarCotizacionListas(CotizacionListas cotizacionListas)
        {
            this._uow.CotizacionListasRepository.UpdateCotizacionListas(cotizacionListas);

            return cotizacionListas;
        }
    }
}
