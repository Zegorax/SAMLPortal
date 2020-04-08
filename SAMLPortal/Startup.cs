using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Util;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using SAMLPortal.Misc;
using SAMLPortal.Models;
using SAMLPortal.Services;

namespace SAMLPortal
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddScoped<IAuthenticationService, LdapAuthenticationService>();

			GlobalSettings.InitSettingsFromEnvironment();

			services.AddDbContext<SAMLPortalContext>(options =>
				options.UseMySql("Server=localhost; Database=samlportal; User=root; Password=root;",
					mysqlOptions =>
					mysqlOptions.ServerVersion(new ServerVersion(new Version(10, 4, 6), ServerType.MySql))
				)
			);

			services.Configure<Saml2Configuration>(saml2Configuration =>
			{

				//saml2Configuration.SigningCertificate = CertificateUtil.Load(AppEnvironment.MapToPhysicalFilePath(Configuration["Saml2:SigningCertificateFile"]), Configuration["Saml2:SigningCertificatePassword"]);
				saml2Configuration.AllowedAudienceUris.Add(saml2Configuration.Issuer);
			});

			services.AddAuthentication("SAMLPortal").AddCookie("SAMLPortal", options =>
			{
				options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Auth/Login");
				options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Auth/Denied");
			});

			services.AddControllersWithViews();

			//var isAdminUserPolicy = new AuthorizationPolicyBuilder().RequireRole(UserRoles.Administrator).Build();
			//services.AddMvc(options =>
			//{
			//    options.Filters.Add(new ApplyPolicyOrAuthorizeFilter(isAdminUserPolicy));
			//});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SAMLPortalContext context)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				context.Database.Migrate();
			}

			GlobalSettings.GenerateSigningCertificate();

			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}");
			});

			//SAMLPortalContext context = new SAMLPortalContext();
			//if (!context.Setup.Any())
			//{
			//    Setup initialSetup = new Setup();
			//    initialSetup.IsConfigured = false;
			//    context.Add(initialSetup);
			//    context.SaveChanges();
			//}

			//App test = new App();
			//test.Name = "Gitlab";
			//test.Description = "An awesome GitHub alternative";
			//test.Enabled = true;
			//using (var context = new SAMLPortalContext())
			//{
			//    //context.Add(test);
			//    //context.SaveChanges();
			//    //Console.WriteLine("CHANGED");

			//    var allApps = context.App.ToList();
			//}
		}
	}
}