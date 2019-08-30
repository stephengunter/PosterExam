using Microsoft.AspNetCore.Authorization;

namespace ApplicationCore.Authorization
{
	public class HasPermissionRequirement : IAuthorizationRequirement
	{
		public string PermissionName { get; private set; }

		public HasPermissionRequirement(string permissionName)
		{
			this.PermissionName = permissionName;
		}

		
	}

}
