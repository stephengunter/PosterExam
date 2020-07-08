using ApplicationCore.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Views;

namespace ApplicationCore.DataAccess
{
	public class AppDBSeed
	{
		public static async Task EnsureSeedData(IServiceProvider serviceProvider)
		{
			Console.WriteLine("Seeding database...");

			using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				var defaultContext = scope.ServiceProvider.GetRequiredService<DefaultContext>();
				defaultContext.Database.Migrate();
				
				var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
				var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

				await SeedRoles(roleManager);
				await SeedUsers(userManager);

			}

			Console.WriteLine("Done seeding database.");
			Console.WriteLine();
		}

		static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
		{
			var roles = new List<string> { "Dev", "Boss", "Subscriber" };
			foreach (var item in roles)
			{
				await AddRoleIfNotExist(roleManager, item);
			}


		}

		static async Task AddRoleIfNotExist(RoleManager<IdentityRole> roleManager, string roleName)
		{
			var role = await roleManager.FindByNameAsync(roleName);
			if (role == null)
			{
				await roleManager.CreateAsync(new IdentityRole { Name = roleName });

			}


		}

		static async Task SeedUsers(UserManager<User> userManager)
		{
			string email = "traders.com.tw@gmail.com";
			var roles = new List<string>() { "Dev" };

			await CreateUserIfNotExist(userManager, email, roles);

			

		}


		static async Task CreateUserIfNotExist(UserManager<User> userManager, string email, IList<string> roles = null)
		{
			var user = await userManager.FindByEmailAsync(email);
			if (user == null)
			{
				bool isAdmin = false;
				if (!roles.IsNullOrEmpty())
				{
					isAdmin = roles.Select(r => r == "Dev" || r == "Boss").FirstOrDefault();
				}

				var newUser = new User
				{
					Email = email,
					UserName = email,


					EmailConfirmed = isAdmin,
					SecurityStamp = Guid.NewGuid().ToString()

				};


				var result = await userManager.CreateAsync(newUser);

				if (!roles.IsNullOrEmpty())
				{
					await userManager.AddToRolesAsync(newUser, roles);
				}


			}
			else
			{
				if (!roles.IsNullOrEmpty())
				{
					foreach (var role in roles)
					{
						bool hasRole = await userManager.IsInRoleAsync(user, role);
						if (!hasRole) await userManager.AddToRoleAsync(user, role);
					}
				}

			}
		}

		
		
	}
}
