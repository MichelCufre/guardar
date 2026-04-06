using NLog;
using WIS.Domain.DataModel;
using WIS.Domain.General;

namespace WIS.Domain.Registro
{
    public class RegistroProductoProveedor
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroProductoProveedor(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        public virtual ProductoProveedor RegistrarProductoProveedor(ProductoProveedor nuevoProductoProveedor)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                Agente agente = this._uow.AgenteRepository.GetAgenteByCodigoAgente(nuevoProductoProveedor.Cliente);
                nuevoProductoProveedor.Cliente = agente.CodigoInterno;
                this._uow.ProductoRepository.AddProductoProveedor(nuevoProductoProveedor);

                return nuevoProductoProveedor;
            }
        }

        public virtual ProductoProveedor UpdateProductoProveedor(ProductoProveedor ProductoProveedor)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                Agente agente = this._uow.AgenteRepository.GetAgenteByCodigoAgente(ProductoProveedor.Cliente);
                ProductoProveedor.Cliente = agente.CodigoInterno;
                this._uow.ProductoRepository.UpdateProductoProveedor(ProductoProveedor);

                return ProductoProveedor;
            }
        }
    }
}

