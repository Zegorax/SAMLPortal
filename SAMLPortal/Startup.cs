using System;
using System.Linq;
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

namespace SAMLPortal
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            GlobalSettings.RefreshSetupSettings();

            if (!GlobalSettings.IsConfigured)
            {
                GlobalSettings.TemporaryPassword = Helpers.GenerateRandomPassword();
                Console.WriteLine("To access SAMLPortal setup wizard, use this generated password : " + GlobalSettings.TemporaryPassword);
            }
            
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<SAMLPortalContext>(options =>
                options.UseMySql("Server=localhost; Database=samlportal; User=root; Password=root;",
                        mysqlOptions =>
                            mysqlOptions.ServerVersion(new ServerVersion(new Version(10,4,6), ServerType.MySql))
                        )
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            SAMLPortalContext context = new SAMLPortalContext();
            if (!context.Setup.Any())
            {
                Setup initialSetup = new Setup();
                initialSetup.IsConfigured = false;
                context.Add(initialSetup);
                context.SaveChanges();
            }

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
