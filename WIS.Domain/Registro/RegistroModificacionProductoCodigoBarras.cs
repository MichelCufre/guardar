using NLog;
using WIS.Domain.DataModel;
using WIS.Domain.General;

namespace WIS.Domain.Registro
{
    public class RegistroModificacionProductoCodigoBarras
    {

        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroModificacionProductoCodigoBarras(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        /// <summary>
        /// + Crea codigo de barra asociado al producto y empresa
        /// </summary>
        /// <param name="productoCodigoBarra">Codigo de barra a crear</param>
        /// <returns>Retorna el codigo de barra con toda la informacion</returns>
        public virtual ProductoCodigoBarra RegistrarCodigoBarra(ProductoCodigoBarra productoCodigoBarra)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {

                this._uow.ProductoCodigoBarraRepository.AgregarCodigoBarra(productoCodigoBarra);

                return productoCodigoBarra;

            }

        }

        /// <summary>
        /// + Actualiza el codigo de barra ingresado
        /// </summary>
        /// <param name="productoCodigoBarra">Codigo de barra a actualizar</param>
        /// <returns>Retorna el codigo de barra modificado</returns>
        public virtual ProductoCodigoBarra ModificarCodigoBarra(ProductoCodigoBarra productoCodigoBarra)
        {
            this._uow.ProductoCodigoBarraRepository.UpdateCodigoBarras(productoCodigoBarra);

            return productoCodigoBarra;
        }

        /// <summary>
        /// + Actualiza un codigo de barra con un nuevo valor en su CD_BARRAS
        /// Borrando el viejo codigo de barra y crea uno nuevo con el nuevo codigo de barra proporcionado
        /// </summary>
        /// <param name="productoCodigoBarraNuevo">Codigo de barra modificado para actualizar</param>
        /// <param name="idCodigoBarraViejo">ID del viejo codigo de barra</param>
        /// <param name="idEmpresaCodigoBarraViejo">ID empresa del viejo codigo de barra</param>
        /// <returns>Retorna el nuevo Codigo de Barras con toda la informacion</returns>
        public virtual ProductoCodigoBarra ModificarCodigoBarra(ProductoCodigoBarra productoCodigoBarraNuevo, string idCodigoBarraViejo, int idEmpresaCodigoBarraViejo)
        {
            var codigoBarraViejo = this._uow.ProductoCodigoBarraRepository.GetProductoCodigoBarra(idCodigoBarraViejo, idEmpresaCodigoBarraViejo);
            codigoBarraViejo.NumeroTransaccion = productoCodigoBarraNuevo.NumeroTransaccion;
            codigoBarraViejo.NumeroTransaccionDelete = productoCodigoBarraNuevo.NumeroTransaccion;

            this._uow.ProductoCodigoBarraRepository.UpdateCodigoBarras(codigoBarraViejo);
            this._uow.SaveChanges();
            this._uow.ProductoCodigoBarraRepository.DeleteCodigoBarra(idCodigoBarraViejo, idEmpresaCodigoBarraViejo);

            this.RegistrarCodigoBarra(productoCodigoBarraNuevo);

            return productoCodigoBarraNuevo;
        }


    }
}
