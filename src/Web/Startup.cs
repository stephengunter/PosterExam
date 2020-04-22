using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NLog;
using ApplicationCore.Authorization;
using ApplicationCore.Middlewares;
using Microsoft.Extensions.Hosting;
using ApplicationCore.DI;

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
			string securityKey = Configuration["AuthSettings:SecurityKey"];
			int tokenValidHours = Convert.ToInt32(Configuration["AuthSettings:TokenValidHours"]);
			services.AddJwtBearer(tokenValidHours, issuer, audience, securityKey);

			services.AddSwagger("PosterExamStarter", "v1");

			services.AddDtoMapper();

			services.AddAuthorization(options =>
			{
				options.AddPolicy(Permissions.Admin.ToString(), policy =>
					policy.Requirements.Add(new HasPermissionRequirement(Permissions.Admin.ToString())));
			});

			services.AddCors(options => options.AddPolicy("api",
			   p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
		    ));
			
			services.AddControllers();

			return AutofacRegister.Register(services);
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
				app.UseMiddleware<ExceptionMiddleware>();
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			


			app.UseCors("api");

			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "PosterExam");
			});
			app.UseSwagger();

			app.UseAuthentication();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});

		}
	}
}
