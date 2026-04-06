using Microsoft.Extensions.Options;
using System;
using WIS.AutomationManager.Interfaces;
using WIS.Domain.Automatismo;
using WIS.Domain.DataModel;
using WIS.Domain.Services.Configuracion;
using WIS.Security;

namespace WIS.AutomationManager.Services
{
    public class AutomatismoEjecucionService : IAutomatismoEjecucionService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected IIdentityService _identity;
        protected readonly IOptions<MaxItemsSettings> _configuration;

        public AutomatismoEjecucionService(IUnitOfWorkFactory uowFactory,
            IOptions<MaxItemsSettings> configuration,
            IIdentityService identity)
        {
            _uowFactory = uowFactory;
            _configuration = configuration;
            _identity = identity;
        }

        public AutomatismoEjecucion CrearEjecucion(int? nuAutomatismo, short cdInterfazExterna, string referencia, int? nuAutomatismoInterfaz, string loginName)
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var ejecucion = new AutomatismoEjecucion();

                ejecucion.Referencia = referencia;
                ejecucion.InterfazExterna = cdInterfazExterna;
                ejecucion.IdAutomatismo = nuAutomatismo;
                ejecucion.IdentityUser = loginName;
                ejecucion.FechaRegistro = DateTime.Now;
                ejecucion.AutomatismoInterfaz = nuAutomatismoInterfaz;
                ejecucion.Id = uow.AutomatismoEjecucionRepository.GetNextNuAutomatismoEjecucion();
                ejecucion.UserId = uow.SecurityRepository.GetUserIdByLoginName(loginName) ?? 0;

                uow.AutomatismoEjecucionRepository.Add(ejecucion);
                uow.SaveChanges();

                return ejecucion;
            }
        }

        public void SetIdentity(string application, int userId)
        {
            this._identity.Application = application;
            this._identity.UserId = userId;
        }

        public void Update(AutomatismoEjecucion obj)
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                uow.AutomatismoEjecucionRepository.Update(obj);
                uow.SaveChanges();
            }
        }
    }
}
