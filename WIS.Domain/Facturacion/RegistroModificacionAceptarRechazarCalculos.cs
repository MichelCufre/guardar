using NLog;
using WIS.Domain.DataModel;

namespace WIS.Domain.Facturacion
{
    public class RegistroModificacionAceptarRechazarCalculos
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroModificacionAceptarRechazarCalculos(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        /// <summary>
        /// + Actualiza la facturacion resultado ingresada
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
