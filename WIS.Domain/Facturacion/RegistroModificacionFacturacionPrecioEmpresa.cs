using NLog;
using WIS.Domain.DataModel;
using WIS.Domain.General;

namespace WIS.Domain.Facturacion
{
    public class RegistroModificacionFacturacionPrecioEmpresa
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroModificacionFacturacionPrecioEmpresa(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        /// <summary>
        /// + Actualiza la empresa ingresada
        /// </summary>
        /// <param name="empresa">Empresa a actualizar</param>
        /// <returns>Retorna la empresa modificada</returns>
        public virtual Empresa ModificarPrecioEmpresa(Empresa empresa)
        {
            this._uow.EmpresaRepository.UpdateEmpresa(empresa);

            return empresa;
        }
    }
}
