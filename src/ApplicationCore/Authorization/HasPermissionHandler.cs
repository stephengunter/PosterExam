using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Helpers;
using ApplicationCore.Services;

namespace ApplicationCore.Authorization
{
	public class HasPermissionHandler : AuthorizationHandler<HasPermissionRequirement>
	{
		IPermissionsService permissionService;
		public HasPermissionHandler(IPermissionsService permissionService)
		{
			this.permissionService = permissionService;
		}

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermissionRequirement requirement)
		{
			bool isDev = context.CurrentUserIsDev();
			if (isDev)
			{
				context.Succeed(requirement);
				return Task.CompletedTask;
			}

			bool isBoss = context.CurrentUserIsBoss();
			if (isBoss)
			{
				context.Succeed(requirement);
				return Task.CompletedTask;
			}

			string permissionName = requirement.PermissionName;
			string userId = context.CurrentUserId();

			if (String.IsNullOrEmpty(userId))
			{
				context.Fail();
				return Task.CompletedTask;
			}


			var roles = context.CurrentUseRoles();


			bool hasPermission = permissionService.IsUserHasPermission(permissionName, userId, roles);

			if (hasPermission) context.Succeed(requirement);
			else context.Fail();


			return Task.CompletedTask;
		}
	}
}
