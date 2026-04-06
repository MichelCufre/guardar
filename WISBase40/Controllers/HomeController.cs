using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.Drawing;
using System.Threading.Tasks;
using WIS.Exceptions;
using WIS.TrafficOfficer;
using WIS.WebApplication.ActionFilters;
using WIS.WebApplication.Models;
using WIS.WebApplication.Models.Managers;

namespace WIS.WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISessionManager _sessionManager;
        private readonly ITrafficOfficerFrontendService _trafficOfficer;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public HomeController(ISessionManager sessionManager,
            ITrafficOfficerFrontendService trafficOfficer)
        {
            this._sessionManager = sessionManager;
            this._trafficOfficer = trafficOfficer;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = this._sessionManager.GetUserInfo();

            if (user == null || !user.IsEnabled)
                return Redirect("/api/Security/Logout");

            if (user.Predios.Count == 0)
                return Redirect("/UserWithoutLocations");

            try
            {
                if (string.IsNullOrEmpty(this._trafficOfficer.SessionToken))
                    await this._trafficOfficer.CreateSession(default);
                else
                    await this._trafficOfficer.UpdateSessionActivity(default);
            }
            catch (TooManySessionsException ex)
            {
                this._logger.Error(ex, ex.Message);
                return Redirect("/TooManySessions");
            }
            catch (ExpiredLicenseException ex)
            {
                this._logger.Error(ex, ex.Message);
                return Redirect("/ExpiredLicense");
            }
            catch (InvalidLicenseException ex)
            {
                this._logger.Error(ex, ex.Message);
                return Redirect("/InvalidLicense");
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
                return Redirect("/api/Security/Logout");
            }

            return Redirect("/Default");
        }
    }
}
