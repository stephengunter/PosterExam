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
				await SeedSubjects(defaultContext);
				await SeedSubSubjects(defaultContext);
				//await SeedTestCategories(defaultContext);

			}

			Console.WriteLine("Done seeding database.");
			Console.WriteLine();
		}

		static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
		{
			var roles = new List<string> { "Dev", "Boss", "Author" };
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
			string name = "何金水";
			var roles = new List<string>() { "Dev" };

			await CreateUserIfNotExist(userManager, email, name, roles);

		}


		static async Task CreateUserIfNotExist(UserManager<User> userManager, string email, string name, IList<string> roles = null)
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
					Name = name,


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

		static async Task SeedSubjects(DefaultContext context)
		{
			var subjects = new List<Subject>
			{
				new Subject { Title = "臺灣自然及人文地理" },
				new Subject { Title = "郵政法大意及交通安全常識"  }
			};

			foreach (var item in subjects)
			{
				var exist = context.Subjects.Where(x => x.Title == item.Title).FirstOrDefault();
				if (exist == null)
				{
					await context.Subjects.AddAsync(item);
				}
			}

			await context.SaveChangesAsync();

		}

		static async Task SeedSubSubjects(DefaultContext context)
		{
			var subjects = new List<SubjectViewModel>
			{
				new SubjectViewModel { Title = "郵政法", ParentTitle = "郵政法大意及交通安全常識" }
			};

			foreach (var item in subjects)
			{
				var parent = context.Subjects.Where(x => x.Title == item.ParentTitle).FirstOrDefault();
				var exist = context.Subjects.Where(x => x.Title == item.Title).FirstOrDefault();
				if (exist == null)
				{
					await context.Subjects.AddAsync(new Subject { Title = item.Title, ParentId = parent.Id });
				}
				else
				{
					exist.ParentId = parent.Id;
				}
			}

			await context.SaveChangesAsync();
		}


		//static async Task SeedTestCategories(DefaultContext context)
		//{
		//	if (context.Categories.Count() > 0) return;

		//	var first = new List<Category>
		//	{
		//		new Category {  Title = "第一章" },
		//		new Category {  Title = "第二章"}
		//	};
		//	foreach (var item in first) await CreateOrUpdateCategory(context, item);

		//	string parent = "第一章";
		//	await CreateOrUpdateCategory(context, new Category { Title = "1-1" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = "1-2" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = "1-3" }, parent);

		//	parent = "第二章";
		//	await CreateOrUpdateCategory(context, new Category { Title = "2-1" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = "2-2" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = "2-3" }, parent);

		//	parent = "1-1";
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-1" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-2" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-3" }, parent);

		//	parent = "1-2";
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-1" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-2" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-3" }, parent);

		//	parent = "2-1";
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-1" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-2" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-3" }, parent);

		//	parent = "2-2";
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-1" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-2" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-3" }, parent);

		//	parent = "2-3";
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-1" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-2" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-3" }, parent);


		//	parent = "1-1-1";
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-1" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-2" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-3" }, parent);

		//	parent = "1-1-2";
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-1" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-2" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-3" }, parent);

		//	parent = "1-1-3";
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-1" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-2" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-3" }, parent);

		//	parent = "1-2-1";
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-1" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-2" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-3" }, parent);

		//	parent = "1-2-2";
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-1" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-2" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-3" }, parent);

		//	parent = "1-2-3";
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-1" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-2" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-3" }, parent);
			

		//	parent = "2-1-1";
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-1" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-2" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-3" }, parent);

		//	parent = "2-1-2";
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-1" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-2" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-3" }, parent);

		//	parent = "2-1-3";
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-1" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-2" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-3" }, parent);

		//	parent = "2-2-1";
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-1" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-2" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-3" }, parent);

		//	parent = "2-2-2";
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-1" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-2" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-3" }, parent);

		//	parent = "2-2-3";
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-1" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-2" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-3" }, parent);

		//	parent = "2-3-1";
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-1" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-2" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-3" }, parent);

		//	parent = "2-3-2";
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-1" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-2" }, parent);
		//	await CreateOrUpdateCategory(context, new Category { Title = parent + "-3" }, parent);

			




		//}

		//static async Task CreateOrUpdateCategory(DefaultContext context, Category category, string parentName = "")
		//{
		//	category.Type = CategoryType.Term;
		//	var exist = context.Categories.Where(x => x.Title == category.Title).FirstOrDefault();

		//	int parentId = 0;
		//	if (!String.IsNullOrEmpty(parentName))
		//	{ 
		//		var parentCategory = context.Categories.Where(x => x.Title == parentName).FirstOrDefault();
		//		parentId = parentCategory.Id;
		//	}

		//	if (exist == null)
		//	{
		//		category.ParentId = parentId;
		//		await context.Categories.AddAsync(category);
		//	}
		//	else
		//	{
		//		exist.ParentId = parentId;
		//	}

		//	await context.SaveChangesAsync();
		//}

	}
}
