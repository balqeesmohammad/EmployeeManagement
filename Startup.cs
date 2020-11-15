using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EmployeeManagement.Models;
using EmployeeManagement.Security;

namespace EmployeeManagement
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));
           // app.UseMvc();
            services.AddMvc(option => {

                option.EnableEndpointRouting = false;
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();//for authorization globally 
                option.Filters.Add(new AuthorizeFilter(policy));

            }).AddXmlSerializerFormatters();
            //external login provider(google )
            services.AddAuthentication().AddGoogle(options => {
                options.ClientId = "162406962715-d7l9b8s6mr1num1nk778hd81d4eaonoq.apps.googleusercontent.com";
                options.ClientSecret = "PIISq1Myou6wAQZkdHeOBwL7";
                //external login provider(facebook)

            }).AddFacebook(options=> {
                options.AppId = "812256476212712";
                options.AppSecret = "15891ec3e49423f2c8b4715cd5e4ae95";
            });
            services.ConfigureApplicationCookie(option => {
                option.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });
            
            services.AddAuthorization(option =>
            {
                //claim policy
            option.AddPolicy("CreateRolePolicy", policy => policy.RequireClaim("Create Role", "true"));
            option.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role"));

      //          option.InvokeHandlersAfterFailure = false;

                //option.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context => 
                //    context.User.IsInRole("admin") &&
                //    context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
                //    context.User.IsInRole("Super Admin")
                //
                //Register for custom handler -Authrization requirement 

                    option.AddPolicy("EditRolePolicy",
                     policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement())
                 





             );
                //role policy
            option.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("admin"));

            });

            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();

            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler > (); //// Register for custom handler -Authrization requirement 
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>(); //// Register for custom handler -Authrization requirement 

            services.AddIdentity<ApplicationUser, IdentityRole>(
                options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequiredUniqueChars = 3;
                    options.Password.RequireNonAlphanumeric = false;
                    options.SignIn.RequireConfirmedEmail = true;

                    // Register((CustomEmailConfirmationTokenProvider)))
                    options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";

                    //lockout properity 
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                }
                ).AddEntityFrameworkStores<AppDbContext>()
                ////built-in asp.net core default token provider 
                .AddDefaultTokenProviders()

            /// Register ((CustomEmailConfirmationTokenProvider)))
            .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation");


            // change token life sapn of all token type 
            // Set token life span to 10 hours
            services.Configure<DataProtectionTokenProviderOptions>(
                o => o.TokenLifespan = TimeSpan.FromHours(10)
            );

            // Changes token lifespan of just the Email Confirmation Token type
            services.Configure<CustomEmailConfirmationTokenProviderOptions>(o =>
                        o.TokenLifespan = TimeSpan.FromDays(5));
            services.AddSingleton<DataProtectionPurposeStrings>();

            //services.Configure<IdentityOptions>(options => {
            //    options.Password.RequiredLength = 10;
            //    options.Password.RequiredUniqueChars = 3;
            //    options.Password.RequireNonAlphanumeric = false;
            //                                   } );
            //  services.AddMvcOptions.EnableEndpointRouting = 'false';
            //services.MvcOptions.EnableEndpointRouting = 'false';
            // services.
        }//dependency injection container
        
        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            //, ILogger<Startup> logger
            if (env.IsDevelopment()) 
            {
                //DeveloperExceptionPageOptions developerExceptionPageOptions = new DeveloperExceptionPageOptions
                //{
                //    SourceCodeLineCount = 10 

                //};

                app.UseDeveloperExceptionPage();
            }
            else if (env.IsProduction() || env.IsStaging() || env.IsEnvironment("Staging_2"))
           
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");

            }
            //app.UseExceptionHandler("/Error");
            app.UseRouting();
          //  app.UseStatusCodePagesWithReExecute("/Error/{0}");

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});


                //app.Use(async (context, next) =>
                //{
                //    logger.LogInformation("MW1: Incomming request");
                //    await next();
                //    logger.LogInformation("MW1 outgoing response ");
                //});

                //app.Use(async (context, next) =>
                //{
                //    logger.LogInformation("MW2: Incomming request");
                //    await next();
                //    logger.LogInformation("MW2 outgoing response ");

                //});

                //DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
                //defaultFilesOptions.DefaultFileNames.Clear();
                //defaultFilesOptions.DefaultFileNames.Add("foo.html");

                //FileServerOptions fileServerOptions = new FileServerOptions();
                //fileServerOptions.DefaultFilesOptions.DefaultFileNames.Clear();
                //fileServerOptions.DefaultFilesOptions.DefaultFileNames.Add("foo.html");


                //app.UseDefaultFiles(defaultFilesOptions);&& UseDirectoryBrowser
                //app.UseStaticFiles(); =====> instead these 2 mw use just 1 mw in bellow
                /// UseDefaultFiles+UseStaticFiles+UseDirectoryBrowser = UseFileServer (combines between them )

                //app.UseFileServer(fileServerOptions);
                //  app.UseFileServer();
                // app.UseStatusCodePagesWithRedirects("/Error/{0}");

                app.UseStaticFiles();
                //app.UseMvcWithDefaultRoute();
                app.UseAuthentication();
                app.UseCors("enableCors");

                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name:"default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                });
                

                //     app.UseMvc();

                

            });
        }
    }
}
