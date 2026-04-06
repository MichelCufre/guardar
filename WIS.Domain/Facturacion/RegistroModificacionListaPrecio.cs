using NLog;
using WIS.Domain.DataModel;

namespace WIS.Domain.Facturacion
{
    public class RegistroModificacionListaPrecio
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroModificacionListaPrecio(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        /// <summary>
        /// + Crea un dato de lista de precio
        /// </summary>
        /// <param name="listaPrecio">Lista precio a crear</param>
        /// <returns>Retorna la lista con toda la informacion</returns>
        public virtual ListaPrecio RegistrarListaPrecio(ListaPrecio listaPrecio)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.ListaPrecioRepository.AddListaPrecio(listaPrecio);

                return listaPrecio;
            }
        }

        /// <summary>
        /// + Actualiza el dato de lista de precio ingresado
        /// </summary>
        /// <param name="listaPrecio">Lista precio a actualizar</param>
        /// <returns>Retorna la lista modificada</returns>
        public virtual ListaPrecio ModificarListaPrecio(ListaPrecio listaPrecio)
        {
            this._uow.ListaPrecioRepository.UpdateListaPrecio(listaPrecio);

            return listaPrecio;
        }
    }
}
