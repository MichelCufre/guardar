using NLog;
using WIS.Domain.DataModel;

namespace WIS.Domain.Recepcion
{
    public class MantenimientoPanelDeEstrategias
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public MantenimientoPanelDeEstrategias(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }


        public virtual Estrategia RegistrarEstrategia(Estrategia estrategia)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.EstrategiaRepository.AddEstrategia(estrategia);

                return estrategia;
            }
        }
        public virtual InstanciaLogica RegistrarInstanciaLogica(InstanciaLogica instanciaLogica)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.EstrategiaRepository.AddInstanciaLogica(instanciaLogica);

                return instanciaLogica;
            }
        }
        public virtual InstanciaLogicaParametro RegistrarInstanciaParametro(InstanciaLogicaParametro instanciaParametro)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.EstrategiaRepository.AddInstanciaParametro(instanciaParametro);

                return instanciaParametro;
            }
        }

        public virtual AsociacionEstrategia RegistrarAsociacionEstrategia(AsociacionEstrategia asociacionEstrategia)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.EstrategiaRepository.AddAsociacionEstrategia(asociacionEstrategia);

                return asociacionEstrategia;
            }
        }

        public virtual Estrategia UpdateEstrategia(Estrategia estrategia)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.EstrategiaRepository.UpdateEstrategia(estrategia);

                return estrategia;
            }
        }

        public virtual InstanciaLogica UpdateInstanciaLogica(InstanciaLogica instanciaLogica)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.EstrategiaRepository.UpdateInstanciaLogica(instanciaLogica);

                return instanciaLogica;
            }
        }
        public virtual InstanciaLogicaParametro UpdateInstanciaParametro(InstanciaLogicaParametro instanciaParametro)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.EstrategiaRepository.UpdateInstanciaParametro(instanciaParametro);

                return instanciaParametro;
            }
        }

        public virtual Estrategia BorrarEstrategia(Estrategia estrategia)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.EstrategiaRepository.DeleteEstrategia(estrategia);

                return estrategia;
            }
        }

        public virtual InstanciaLogica BorrarInstancia(InstanciaLogica instancia)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.EstrategiaRepository.DeleteInstancia(instancia);

                foreach (var instanciaPosterior in this._uow.EstrategiaRepository.GetInstanciasPosteriores(instancia))
                {
                    instanciaPosterior.Orden--;
                    this._uow.EstrategiaRepository.UpdateEstrategiaInstancia(instanciaPosterior);
                }

                return instancia;
            }  
        }

        public virtual AsociacionEstrategia BorrarAsociacionEstrategia(AsociacionEstrategia asociacionEstrategia)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.EstrategiaRepository.DeleteAsociacionEstrategia(asociacionEstrategia);

                return asociacionEstrategia;
            }
        }
    }
}
