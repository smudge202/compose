﻿using Microsoft.Framework.DependencyInjection;
using System;

namespace Compose
{
    public static class ApplicationExtensions
    {
		public static void UseServices(this Application app, Action<IServiceCollection> configureServices)
		{
			app.UseServices(services =>
			{
				configureServices(services);
				return app.CreateProvider(app.Provider, services);
			});
		}

		internal static void UseServices(this Application app, Func<IServiceCollection, WrappedReflectionServiceProvider> configureServices)
		{
			app.Provider = configureServices(app.Services);
		}

		public static bool Transition<TService, TImplementation>(this Application app) where TImplementation : class, TService
		{
			var transitional = app.HostingServices.GetService<TService>() as IDirectTransition<TService>;
			if (transitional == null) throw new InvalidOperationException($"{typeof(TService).Name} must be registered as a Transitional Service (services.AddTransitional<{typeof(TService).Name}, TImplementation>()");
			return transitional.Change(app.GetRequiredService<TImplementation>());
		}
	}
}