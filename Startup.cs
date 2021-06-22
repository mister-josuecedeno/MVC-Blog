using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MVC_Blog.Data;
using MVC_Blog.Models;
using MVC_Blog.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Blog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // database
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseNpgsql(
            //        Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    Connection.GetConnectionString(Configuration)));

            // CORS Policy (This is the MOST liberal)
            // Enhancement - limit it to a white list
            services.AddCors(options => 
            {
                options.AddPolicy("DefaultPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddDatabaseDeveloperPageExceptionFilter();

            // services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            services.AddIdentity<BlogUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddDefaultUI()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            
            services.AddControllersWithViews();
            
            services.AddRazorPages();

            // Add file/image service (interface + concrete class)
            services.AddScoped<IFileService, BasicFileService>();

            // Add Data Service (for seeding users)
            services.AddScoped<DataService>();

            // Add Basic Slug Service
            services.AddScoped<BasicSlugService>();

            // Add service using an existing interface
            services.AddScoped<IEmailSender, GmailEmailService>();

            // Add a search service
            services.AddScoped<SearchService>();

            // Add Swagger Service 
            services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments($"{Directory.GetCurrentDirectory()}/wwwroot/MVC-Blog.xml", true);
                c.SwaggerDoc("v1", new OpenApiInfo { 
                    Title = "MVC Blog API", 
                    Version = "v1", 
                    Description = "Serving up blog data using .Net Core",
                    Contact = new OpenApiContact
                    {
                        Name = "Josue Cedeno",
                        Email = "josuecedeno@gmail.com",
                        Url = new System.Uri("https://www.linkedin.com/in/josuecedeno/")
                    }
                });
            
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // CORS Policy
            app.UseCors("DefaultPolicy");

            // Swagger Config
            app.UseSwagger();
            app.UseSwaggerUI(c => 
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TestAPI v1");
                c.InjectJavascript("/swagger/swagger.js");
                c.InjectStylesheet("/swagger/swagger.css");
                c.DocumentTitle = "MVC Blog Public API";
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
            endpoints.MapControllerRoute(
                name: "SEO_Route",
                pattern: "JosuesPosts/SEO/{slug}",
                defaults: new { controller = "Posts", action = "Details" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
