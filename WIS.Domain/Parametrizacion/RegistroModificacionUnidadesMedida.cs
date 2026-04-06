using NLog;
using WIS.Domain.DataModel;
using WIS.Domain.General;

namespace WIS.Domain.Parametrizacion
{
    public class RegistroModificacionUnidadesMedida
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroModificacionUnidadesMedida(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        /// <summary>
        /// + Crea unidad de medida
        /// </summary>
        /// <param name="unidadMedida">Unidad de medida a crear</param>
        /// <returns>Retorna la unidad con toda la informacion</returns>
        public virtual UnidadMedida RegistrarUnidadesMedida(UnidadMedida unidadMedida)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.UnidadMedidaRepository.AddUnidadMedida(unidadMedida);

                return unidadMedida;
            }
        }

        /// <summary>
        /// + Actualiza la unidad de medida ingresada
        /// </summary>
        /// <param name="unidadMedida">Unidad de medida a actualizar</param>
        /// <returns>Retorna la unidad de medida modificada</returns>
        public virtual UnidadMedida ModificarUnidadesMedida(UnidadMedida unidadMedida)
        {
            this._uow.UnidadMedidaRepository.UpdateUnidadMedida(unidadMedida);

            return unidadMedida;
        }
    }
}
