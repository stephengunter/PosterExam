using Autofac;
using Module = Autofac.Module;
using Autofac.Core.Activators.Reflection;
using System;
using System.Reflection;
using System.Linq;
using ApplicationCore.Auth;
using ApplicationCore.Logging;
using ApplicationCore.DataAccess;
using ApplicationCore.Services;
using ApplicationCore.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace ApplicationCore
{
	public class Modules : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<JwtFactory>().As<IJwtFactory>().SingleInstance().FindConstructorsWith(new InternalConstructorFinder());
			builder.RegisterType<JwtTokenHandler>().As<IJwtTokenHandler>().SingleInstance().FindConstructorsWith(new InternalConstructorFinder());
			builder.RegisterType<TokenFactory>().As<ITokenFactory>().SingleInstance();
			builder.RegisterType<JwtTokenValidator>().As<IJwtTokenValidator>().SingleInstance().FindConstructorsWith(new InternalConstructorFinder());

			builder.RegisterType<Logger>().As<ILogger>().SingleInstance();

			builder.RegisterType<PermissionsService>().As<IPermissionsService>().InstancePerLifetimeScope();
			builder.RegisterType<HasPermissionHandler>().As<IAuthorizationHandler>().InstancePerLifetimeScope();

			builder.RegisterGeneric(typeof(DefaultRepository<>)).As(typeof(IDefaultRepository<>)).InstancePerLifetimeScope();			

			builder.RegisterType<AuthService>().As<IAuthService>().InstancePerLifetimeScope();
			builder.RegisterType<SubjectsService>().As<ISubjectsService>().InstancePerLifetimeScope();
			builder.RegisterType<TermsService>().As<ITermsService>().InstancePerLifetimeScope();

		}
	}

	public class InternalConstructorFinder : IConstructorFinder
	{
		public ConstructorInfo[] FindConstructors(Type targetType)
		{
			return targetType.GetTypeInfo().DeclaredConstructors.Where(c => !c.IsPrivate && !c.IsPublic).ToArray();
		}
	}
}
