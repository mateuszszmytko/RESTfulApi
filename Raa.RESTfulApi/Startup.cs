using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Raa.RESTfulApi.Entities;
using Raa.RESTfulApi.Services;
using Raa.AspNetCore.MongoDataContext;
using Raa.AspNetCore.MongoDataContext.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Raa.RESTfulApi.Models;

namespace Raa.RESTfulApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<JwtSettings>(Configuration.GetSection("Token"));
            services.AddSingleton<IJwtService, JwtService>();


            services.AddMongoDataContext<MongoDataContext>(o =>
            {
                var mongoConf = Configuration.GetSection("MongoConnection");
                o.ConnectionString = mongoConf.GetValue<string>("ConnectionString");
                o.DatabaseName = mongoConf.GetValue<string>("DatabaseName");
            })
                .CreateRepository<UserMessage>();

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddMongoDataStores<MongoDataContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = Configuration["Token:Issuer"],
                        ValidAudience = Configuration["Token:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Token:Key"]))
                    };
                });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddMvc(setupAction => {
                setupAction.ReturnHttpNotAcceptable = true;

                setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                setupAction.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            app.UseAuthentication();

            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder => {
                    appBuilder.Run(async c =>
                    {
                        c.Response.StatusCode = 500;
                        await c.Response.WriteAsync("An unexpected fault.");
                    });
                });
            }

            app.UseCors("CorsPolicy");

            AutoMapper.Mapper.Initialize(cfg =>
            {

                cfg.CreateMap<ApplicationUser, ReturnUserDto>()
                    .ForMember(dto => dto.RegisterDate, opt => opt.MapFrom(a => a.Id.CreationTime))
                    .ForMember(dto => dto.Id, opt => opt.MapFrom(a => a.Id.ToString()));

                cfg.CreateMap<ApplicationUser, UpdateUserDto>()
                    .ForMember(dto => dto.RegisterDate, opt => opt.MapFrom(a => a.Id.CreationTime));
                cfg.CreateMap<UpdateUserDto, ApplicationUser>();

            });



            app.UseMvc();
        }
    }
}
