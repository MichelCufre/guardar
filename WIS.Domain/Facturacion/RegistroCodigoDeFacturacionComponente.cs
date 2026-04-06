using NLog;
using WIS.Domain.DataModel;

namespace WIS.Domain.Facturacion
{
    public class RegistroCodigoDeFacturacionComponente
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroCodigoDeFacturacionComponente(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        public virtual FacturacionCodigoComponente RegistrarCodigoFacturacionComponente(FacturacionCodigoComponente codigoFacutracionComponente)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.FacturacionRepository.AddCodigoFacturacionComponente(codigoFacutracionComponente);

                return codigoFacutracionComponente;
            }
        }

        public virtual FacturacionCodigoComponente UpdateCodigoFacturacionComponente(FacturacionCodigoComponente codigoFacutracionComponente)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.FacturacionRepository.UpdateCodigoFacturacionComponente(codigoFacutracionComponente);

                return codigoFacutracionComponente;
            }
        }

    }
}
