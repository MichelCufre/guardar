using NLog;
using WIS.Domain.DataModel;

namespace WIS.Domain.Facturacion
{
    public class RegistroCodigoDeFacturacion
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroCodigoDeFacturacion(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        public virtual FacturacionCodigo RegistrarCodigoFacturacion(FacturacionCodigo codigoFacutracion)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.FacturacionRepository.AddCodigoFacturacion(codigoFacutracion);

                return codigoFacutracion;
            }
        }
        
        public virtual FacturacionCodigo UpdateCodigoFacturacion(FacturacionCodigo codigoFacutracion)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.FacturacionRepository.UpdateCodigoFacturacion(codigoFacutracion);

                return codigoFacutracion;
            }
        }
    }
}
