using ApplicationCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ApplicationCore.Helpers;

namespace ApplicationCore.Services
{
	public interface IPermissionsService
	{
		bool IsUserHasPermission(string permissionName, string userId, IEnumerable<string> roles = null);
	}

	public class PermissionsService : IPermissionsService
	{
		public bool IsUserHasPermission(string permissionName, string userId, IEnumerable<string> roles = null)
		{
			if (permissionName == Permissions.Admin.ToString()) return CanAdmin(userId, roles);

			return false;
		}

		bool HasRole(string roleName, IEnumerable<string> roles)
		{
			var match = roles.Where(r => r == "").FirstOrDefault();
			return match != null;
		}

		bool CanAdmin(string userId, IEnumerable<string> roles = null)
		{
			if (roles.IsNullOrEmpty()) return false;
			var match = roles.Where(r => r == "Dev" || r == "Boss").FirstOrDefault();
			return match != null;
		}
	}
}
