using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Settings;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog;
using ApplicationCore.Auth;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ApplicationCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using ApplicationCore.Services;
using AutoMapper;
using ApplicationCore.DtoMapper;
using Microsoft.AspNetCore.Authorization;
using Web.Helpers;
using ApplicationCore.Middlewares;

namespace Web
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			var nLogConfigPath = string.Concat(Directory.GetCurrentDirectory(), "/nlog.config");
			if (File.Exists(nLogConfigPath)) { LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config")); }
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<DefaultContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("Default"),
				b => b.MigrationsAssembly("ApplicationCore"))
			);

			services.AddIdentity<User, IdentityRole>(options =>
			{
				options.User.RequireUniqueEmail = true;

			})
			.AddEntityFrameworkStores<DefaultContext>()
			.AddDefaultTokenProviders();

			services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
			services.Configure<RootSubjectSettings>(Configuration.GetSection("RootSubjectSettings"));
			services.Configure<AuthSettings>(Configuration.GetSection("AuthSettings"));
			services.Configure<AdminSettings>(Configuration.GetSection("AdminSettings"));
			services.Configure<MongoDBSettings>(Configuration.GetSection("MongoDBSettings"));

			var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["AuthSettings:SecurityKey"]));

			string issuer = Configuration["AppSettings:Name"];
			string audience = Configuration["AppSettings:Url"];

			services.Configure<JwtIssuerOptions>(options =>
			{
				options.Issuer = issuer;
				options.Audience = audience;
				options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
			});

			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = issuer,

				ValidateAudience = true,
				ValidAudience = audience,

				ValidateIssuerSigningKey = true,
				IssuerSigningKey = signingKey,

				RequireExpirationTime = false,
				ValidateLifetime = true,
				ClockSkew = TimeSpan.Zero
			};

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

			})
			.AddJwtBearer(configureOptions =>
			{
				configureOptions.ClaimsIssuer = issuer;
				configureOptions.TokenValidationParameters = tokenValidationParameters;
				configureOptions.SaveToken = true;

				configureOptions.Events = new JwtBearerEvents
				{
					OnAuthenticationFailed = context =>
					{
						if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
						{
							context.Response.Headers.Add("Token-Expired", "true");
						}
						return Task.CompletedTask;
					}
				};
			});

			

			services.AddCors(options => options.AddPolicy("api",
				p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
			));

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "PosterExamStarter", Version = "v1" });
				c.OperationFilter<SwaggerFileUploadFilter>();
				c.AddSecurityDefinition("Bearer", new ApiKeyScheme
				{
					In = "header",
					Description = "Please insert JWT with Bearer into field",
					Name = "Authorization",
					Type = "apiKey"
				});

				c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
				{
					{ "Bearer", new string[] { } }
				});
			});
			

			IMapper mapper = MappingConfig.CreateConfiguration().CreateMapper();
			services.AddSingleton(mapper);


			services.AddAuthorization(options =>
			{
				options.AddPolicy(Permissions.Admin.ToString(), policy =>
					policy.Requirements.Add(new HasPermissionRequirement(Permissions.Admin.ToString())));
			});

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


			// Now register our services with Autofac container.
			var builder = new ContainerBuilder();

			builder.RegisterModule(new ApplicationCore.Modules());

			builder.Populate(services);
			var container = builder.Build();
			// Create the IServiceProvider based on the container.
			return new AutofacServiceProvider(container);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseMiddleware<ExceptionMiddleware>();
				app.UseHsts();
			}

			app.UseCors("api");

			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "PosterExamStarter");
			});
			app.UseSwagger();

			app.UseAuthentication();

			

			app.UseHttpsRedirection();
			app.UseMvc();
		}
	}
}
