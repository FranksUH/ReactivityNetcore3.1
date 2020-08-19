using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using MediatR;
using Application.Activities;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Hosting;
using Domain;
using Microsoft.AspNetCore.Identity;
using Application.Interfaces;
using SecurityInfrastructure.Security;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using AutoMapper;
using Infrastructure.Photo;
using Infrastructure.Interfaces;
using Web.SignalR;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json.Schema;
using System.Threading.Tasks;

namespace Web
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
            services.AddDbContext<DataContext>(opt=>
            {
                opt.UseLazyLoadingProxies();
                opt.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", (policy) =>
                {
                    policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:3000").AllowCredentials();
                });
            });

            services.AddRouting();
            services.AddSignalR();
            services.AddMvc(opt=> {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            })
                .AddFluentValidation(cnf => cnf.RegisterValidatorsFromAssemblyContaining<Create>())
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddMvcOptions(opt => opt.EnableEndpointRouting = false);

            //Identity
            var builder = services.AddIdentityCore<AppUser>();
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
            identityBuilder.AddEntityFrameworkStores<DataContext>();
            identityBuilder.AddSignInManager<SignInManager<AppUser>>();

            //Authentication
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Supper S3cret K31"));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
                                context.Token = accessToken;
                            return Task.CompletedTask;
                        }
                    };

            });

            //Authorization
            services.AddAuthorization(opt=> 
            {
                opt.AddPolicy("IsActivityHost", policy => 
                {
                    policy.Requirements.Add(new IsHostRequirement());
                });
            });
            services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();

            //JWT 
            services.AddScoped<IJWTGenerator, TokenCreator>();
            services.AddScoped<IUserAccesor, UserAccesor>();

            //MediatR
            services.AddMediatR(typeof(List.Handler).Assembly);

            //AutoMapper
            services.AddAutoMapper(typeof(List.Handler));

            //Configuration
            services.Configure<PhotoSettings>(Configuration.GetSection("PhotoConf"));

            //Photo
            services.AddScoped<IPhotoAccesor, PhotoAccesor>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,  IWebHostEnvironment env)
        {            
            app.UseMiddleware<ErrorHandlingMiddleware.ErrorHandlingMiddleware>();
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            app.UseRouting();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
            app.UseSignalR(routes => 
            {
                routes.MapHub<ChatHub>("/chat");
            });
            app.UseMvc();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Fallback}/{action=Index}/{id?}");

                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}
