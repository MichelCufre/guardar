using WIS.Security;
using WIS.Session;
using WIS.TrafficOfficer;

namespace WIS.Application.Setup
{
    public class ApplicationSetupService : IApplicationSetupService
    {
        protected readonly ITrafficOfficerService _trafficOfficerService;
        protected readonly IIdentityService _securityService;
        protected readonly IUserProvider _userProvider;
        protected readonly ISessionAccessor _session;

        public ApplicationSetupService(ITrafficOfficerService trafficOfficerService, IIdentityService securityService, IUserProvider userProvider, ISessionAccessor session)
        {
            this._trafficOfficerService = trafficOfficerService;
            this._securityService = securityService;
            this._userProvider = userProvider;
            this._session = session;
        }

        public virtual void SetupServices(ApplicationSetupInfo data)
        {
            var manager = (IIdentityServiceManager)this._securityService;
            var trafficOfficerManager = (ITrafficOfficerServiceManager)this._trafficOfficerService;
            var sessionManager = (ISessionAccessorManager)this._session;

            manager.SetUser(this._userProvider.GetUserData(data.User), data.Application, data.Predio);
            trafficOfficerManager.SetPageToken(data.Token);
            sessionManager.SetInnerDictionary(data.Session);
        }
    }
}
