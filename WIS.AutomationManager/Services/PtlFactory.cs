using System;
using WIS.AutomationManager.Interfaces;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.Services.Interfaces;
using WIS.Security;

namespace WIS.AutomationManager.Services
{
    public class PtlFactory : IPtlFactory
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IUnitOfWorkInMemoryFactory _uowMemoryFactory;
        protected readonly IPtlInterpreterClientService _interpretService;
        protected readonly IAutomatismoWmsApiClientService _wmsApiClientService;
		private readonly IIdentityService _identity;


		public PtlFactory(IUnitOfWorkFactory uowFactory,
            IUnitOfWorkInMemoryFactory uowMemoryFactory,
            IPtlInterpreterClientService interpretService,
            IAutomatismoWmsApiClientService wmsApiClientService,
			IIdentityService identity)
        {
            _uowFactory = uowFactory;
            _uowMemoryFactory = uowMemoryFactory;
            _interpretService = interpretService;
            _wmsApiClientService = wmsApiClientService;
			_identity = identity;
		}

		public IPtl GetPtl(IAutomatismo automatismo)
        {
            var ptl = (AutomatismoPtl)automatismo;

            return ptl;
        }

        public IPtlService GetService(IPtl ptl)
        {
            switch (ptl.GetTipo())
            {
                case PtlTipoDb.SEPARACION_DOS_FASES:
                    return new PtlSeparacionEnDosFasesService(_uowFactory, _uowMemoryFactory, _interpretService, _wmsApiClientService, ptl, _identity);

                case PtlTipoDb.CROSS_DOCKING_UNA_FASE:
                    return new PtlCrossDockingEnUnaFaseService(_uowFactory, _uowMemoryFactory, _interpretService, _wmsApiClientService, ptl, _identity);

                case PtlTipoDb.PICKING:
                    return new PtlPickingService(_uowFactory, _uowMemoryFactory, _interpretService, _wmsApiClientService, ptl, _identity);

            }

            throw new NotImplementedException();
        }

        public IPtlService GetService(string tipoPtl)
        {
            switch (tipoPtl)
            {
                case PtlTipoDb.SEPARACION_DOS_FASES:
                    return new PtlSeparacionEnDosFasesService(_uowFactory, _uowMemoryFactory, _interpretService, _wmsApiClientService, null, _identity);

                case PtlTipoDb.CROSS_DOCKING_UNA_FASE:
                    return new PtlCrossDockingEnUnaFaseService(_uowFactory, _uowMemoryFactory, _interpretService, _wmsApiClientService, null, _identity);

                case PtlTipoDb.PICKING:
                    return new PtlPickingService(_uowFactory, _uowMemoryFactory, _interpretService, _wmsApiClientService, null, _identity);

            }

            throw new NotImplementedException();
        }
    }
}
