using NLog;
using System.Linq;
using WIS.Domain.DataModel;

namespace WIS.Domain.Recepcion.RecepcionAgendamiento
{
    public class RecepcionAutomatica
    {
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly IUnitOfWork _uow;
        protected string _aplicacion;
        protected int _usuario;
        protected int _numeroAgenda;

        public RecepcionAutomatica(IUnitOfWork uow, int usuario, string aplicacion, int numeroAgenda)
        {
            this._uow = uow;
            this._usuario = usuario;
            this._aplicacion = aplicacion;
            this._numeroAgenda = numeroAgenda;
        }

        public virtual bool PuedeSerRecibidaAutomaicamente()
        {
            var agenda = _uow.AgendaRepository.GetAgenda(_numeroAgenda);
            var tipoRecepcion = _uow.RecepcionTipoRepository.GetRecepcionTipo(agenda.TipoRecepcionInterno);

            return agenda.EnEstadoAbierta() && tipoRecepcion.PermiteRecepcionAutomatica && agenda.Detalles.Any();
        }

    }
}
