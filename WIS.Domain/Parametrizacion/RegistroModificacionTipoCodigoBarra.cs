using NLog;
using WIS.Domain.DataModel;
using WIS.Domain.General;

namespace WIS.Domain.Parametrizacion
{
    public class RegistroModificacionTipoCodigoBarra
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroModificacionTipoCodigoBarra(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        /// <summary>
        /// + Crea tipo de codigo de barras
        /// </summary>
        /// <param name="productoCodigoBarraTipo">Tipo de codigo de barra a crear</param>
        /// <returns>Retorna el tipo con toda la informacion</returns>
        public virtual ProductoCodigoBarraTipo RegistrarTipoCodigoBarra(ProductoCodigoBarraTipo productoCodigoBarraTipo)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.ProductoCodigoBarraRepository.AgregarTipoCodigoBarra(productoCodigoBarraTipo);

                return productoCodigoBarraTipo;
            }
        }

        /// <summary>
        /// + Actualiza el tipo de codigo de barras ingresado
        /// </summary>
        /// <param name="productoCodigoBarraTipo">Tipo de codigo de barras a actualizar</param>
        /// <returns>Retorna el tipo de codigo de barras modificado</returns>
        public virtual ProductoCodigoBarraTipo ModificarTipoCodigoBarra(ProductoCodigoBarraTipo productoCodigoBarraTipo)
        {
            this._uow.ProductoCodigoBarraRepository.UpdateTipoCodigoBarras(productoCodigoBarraTipo);

            return productoCodigoBarraTipo;
        }
    }
}
