using NLog;
using WIS.Domain.DataModel;
using WIS.Domain.General;

namespace WIS.Domain.Registro
{
    public class RegistroModificacionCodigoSAC
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroModificacionCodigoSAC(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        /// <summary>
        /// + Crea codigo SAC
        /// </summary>
        /// <param name="codigoSAC">Codigo SAC a crear</param>
        /// <returns>Retorna el codigo con toda la informacion</returns>
        public virtual CodigoNomenclaturaComunMercosur RegistrarCodigoSAC(CodigoNomenclaturaComunMercosur codigoSAC)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.NcmRepository.AddNCM(codigoSAC);

                return codigoSAC;
            }
        }

        /// <summary>
        /// + Actualiza el codigo SAC ingresado
        /// </summary>
        /// <param name="codigoSAC">Codigo SAC a actualizar</param>
        /// <returns>Retorna el codigo SAC modificado</returns>
        public virtual CodigoNomenclaturaComunMercosur ModificarCodigoSAC(CodigoNomenclaturaComunMercosur codigoSAC)
        {
            this._uow.NcmRepository.UpdateNCM(codigoSAC);

            return codigoSAC;
        }
    }
}
