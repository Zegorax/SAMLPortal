using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SAMLPortal.Models;

namespace SAMLPortal.Middlewares
{
	public class SetupAssistantMiddleware
	{
		private readonly RequestDelegate _next;

		public SetupAssistantMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public Task Invoke(HttpContext httpContext)
		{
			var configurationStep = GlobalSettings.GetInt("CONFIG_SETUPASSISTANT_STEP");
			string requestPath = httpContext.Request.Path.ToString().ToLower();

			if (configurationStep < 6)
			{
				var endpoint = httpContext.GetEndpoint();
				if (endpoint != null)
				{
					if (!endpoint.DisplayName.StartsWith("SAMLPortal.Controllers.SetupController") && !endpoint.DisplayName.StartsWith("SAMLPortal.Controllers.API.SetupController"))
					{
						httpContext.Response.Redirect("/Setup/" + configurationStep);
					}
				}
				else
				{
					httpContext.Response.Redirect("/Setup/" + configurationStep);
				}
			}

			return _next(httpContext);
		}
	}

	public static class SetupAssistantMiddlewareExtensions
	{
		public static IApplicationBuilder UseSetupAssistantMiddleware(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<SetupAssistantMiddleware>();
		}
	}
}