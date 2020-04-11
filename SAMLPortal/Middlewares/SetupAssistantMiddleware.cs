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

			if (!httpContext.Request.Path.ToString().Contains("Setup") && configurationStep < 6)
			{
				httpContext.Response.Redirect("/Setup/" + configurationStep);
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