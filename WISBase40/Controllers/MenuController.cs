using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using System;
using System.Collections.Generic;
using WIS.WebApplication.ActionFilters;
using WIS.WebApplication.Models.Managers;
using WIS.WebApplication.Models.Menu;

namespace WIS.WebApplication.Controllers
{
    public class MenuController : Controller
    {
        private readonly ISessionManager _sessionManager;
        private readonly IMenuItemProvider _itemProvider;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public MenuController(ISessionManager sessionManager, IMenuItemProvider itemProvider)
        {
            this._sessionManager = sessionManager;
            this._itemProvider = itemProvider;
        }

        [HttpPost]
        [Authorize, CheckAuthorization]
        public IActionResult GetMenuItems()
        {
            try
            {
                var user = this._sessionManager.GetUserInfo();

                if (user == null)
                    return Redirect("/api/Security/Logout");

                List<MenuItem> items = new List<MenuItem>();

                if (user.Predios.Count > 0)
                {
                    items = this._itemProvider.GetItems(user);
                }

                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                return Content(JsonConvert.SerializeObject(items, settings), "application/json");

            }
            catch (Exception ex)
            {
                logger.Error(ex, "WebApplication MenuController - GetMenuItems");
                return null;
            }
        }
    }
}