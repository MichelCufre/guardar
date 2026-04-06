using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.Services;

namespace WIS.AutomationManager.Services
{
    public class AutomatismoServiceContext : ServiceContext
    {
        public Dictionary<string, IAutomatismo> ZonaAutomatismo = new Dictionary<string, IAutomatismo>();
        public Dictionary<string, IAutomatismo> CodigoAutomatismo = new Dictionary<string, IAutomatismo>();
        public Dictionary<string, IAutomatismo> PuestoAutomatismo = new Dictionary<string, IAutomatismo>();
        public Dictionary<string, AutomatismoInterfaz> Interfaces = new Dictionary<string, AutomatismoInterfaz>();

        public AutomatismoServiceContext(IUnitOfWork uow) : base(uow, 0, 0)
        {

        }

        public async override Task Load()
        {
            await base.Load();

            var automatismos = _uow.AutomatismoRepository.GetAutomatismos();

            foreach (var automatismo in automatismos)
            {
                if (automatismo != null)
                {
                    ZonaAutomatismo[automatismo.ZonaUbicacion] = automatismo;
                    CodigoAutomatismo[automatismo.Codigo] = automatismo;
                    foreach (AutomatismoPuesto puesto in automatismo.Puestos)
                    {
                        PuestoAutomatismo[puesto.Puesto] = automatismo;
                    }
                }
            }

            foreach (var interfaz in _uow.AutomatismoInterfazRepository.GetAutomatismosInterfaz())
            {
                Interfaces[GetKey(interfaz)] = interfaz;
            }
        }

        private string GetKey(AutomatismoInterfaz interfaz)
        {
            return string.Format("{0}${1}", interfaz.IdAutomatismo, interfaz.Interfaz);
        }

        protected virtual string GetKeyAutomatismoInterfaz(string nuAutomatismo, int cdInterfaz)
        {
            return string.Format("{0}${1}", nuAutomatismo, cdInterfaz);
        }

        public virtual bool ExisteAutomatismo(string codigo)
        {
            return this.ZonaAutomatismo.GetValueOrDefault(codigo) != null;
        }

        public virtual bool ExisteAutomatismoByCodigo(string codigo)
        {
            return this.CodigoAutomatismo.GetValueOrDefault(codigo) != null;
        }

        public virtual bool ExisteAutomatismoByPuesto(string puesto)
        {
            return this.PuestoAutomatismo.GetValueOrDefault(puesto) != null;
        }

        public virtual IAutomatismo GetAutomatismoZona(string zona)
        {
            return this.ZonaAutomatismo[zona];
        }

        public virtual IAutomatismo GetAutomatismoCodigo(string codigo)
        {
            return this.CodigoAutomatismo[codigo];
        }

        public virtual IAutomatismo GetAutomatismoPuesto(string puesto)
        {
            return this.PuestoAutomatismo[puesto];
        }

        public virtual AutomatismoInterfaz GetAutomatismoInterfaz(IAutomatismo automatismo, int cdInterfaz)
        {
            return this.Interfaces.GetValueOrDefault(this.GetKeyAutomatismoInterfaz(automatismo.Numero.ToString(), cdInterfaz));
        }
    }
}
