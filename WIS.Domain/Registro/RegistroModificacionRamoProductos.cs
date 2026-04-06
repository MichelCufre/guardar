using NLog;
using WIS.Domain.DataModel;
using WIS.Domain.General;

namespace WIS.Domain.Registro
{
    public class RegistroModificacionRamoProductos
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroModificacionRamoProductos(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        /// <summary>
        /// + Crea Ramo de productos
        /// </summary>
        /// <param name="ramoProducto">Ramo de procuto a crear</param>
        /// <returns>Retorna el ramo con toda la informacion</returns>
        public virtual ProductoRamo RegistrarRamoProducto(ProductoRamo ramoProducto)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {

                this._uow.ProductoRamoRepository.AddProductoRamo(ramoProducto);

                return ramoProducto;

            }

        }

        /// <summary>
        /// + Actualiza el ramo de producto ingresado
        /// </summary>
        /// <param name="ramoProducto">Ramo de procuto a actualizar</param>
        /// <returns>Retorna el ramo de producto modificado</returns>
        public virtual ProductoRamo ModificarRamoProducto(ProductoRamo ramoProducto)
        {
            this._uow.ProductoRamoRepository.UpdateRamoProducto(ramoProducto);

            return ramoProducto;
        }




    }
}