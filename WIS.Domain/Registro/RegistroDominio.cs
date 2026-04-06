using NLog;
using WIS.Domain.DataModel;
using WIS.Domain.General;

namespace WIS.Domain.Registro
{
    public class RegistroDominio
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroDominio(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }
        public virtual DominioDetalle RegistrarDetalleDominio(DominioDetalle nuevoDominio)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.DominioRepository.AddDetalleDominio(nuevoDominio);

                return nuevoDominio;
            }
        }

        public virtual DominioDetalle UpdateDetalleDominio(DominioDetalle dominio)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.DominioRepository.UpdateDetalleDominio(dominio);

                return dominio;
            }
        }
    }
}
