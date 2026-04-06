using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.WebApplication.Middlewares
{
    public class RedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public RedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Session != null)
            {
                var path = context.Request.Path.ToUriComponent();

                string token = context.Request.Query["t"];

                if (token != null && !path.Contains("sockjs-node"))
                {
                    var items = context.Request.Query.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();

                    items.RemoveAll(x => x.Key == "t");

                    var qb = new QueryBuilder(items);

                    var absoluteUri = string.Concat(
                    context.Request.Scheme,
                    "://",
                    context.Request.Host.ToUriComponent(),
                    context.Request.PathBase.ToUriComponent(),
                    context.Request.Path.ToUriComponent());

                    string url = absoluteUri + qb.ToString();

                    //SETEAR EL USUARIO
                    //TODO: REVISAR CON MAURO

                    context.Response.Redirect(url);
                }
            }

            await _next.Invoke(context);
        }
    }
}