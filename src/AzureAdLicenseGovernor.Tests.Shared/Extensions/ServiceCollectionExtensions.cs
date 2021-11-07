using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;


namespace AzureAdLicenseGovernor.Tests.Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ReplaceTransient<TService, TImplementation>(this IServiceCollection serviceCollection)
        {
            return serviceCollection.Replace<TService, TImplementation>(ServiceLifetime.Transient);
        }

        public static IServiceCollection ReplaceSingleton<TService, TImplementation>(this IServiceCollection serviceCollection)
        {
            return serviceCollection.Replace<TService, TImplementation>(ServiceLifetime.Singleton);
        }

        public static IServiceCollection ReplaceTransient<TService>(this IServiceCollection serviceCollection, Func<IServiceProvider, TService> factory)
        {
            return serviceCollection.Replace<TService>(provider => factory.Invoke(provider), ServiceLifetime.Transient);
        }

        public static IServiceCollection ReplaceSingleton<TService>(this IServiceCollection serviceCollection, Func<IServiceProvider, TService> factory)
        {
            return serviceCollection.Replace<TService>(provider => factory.Invoke(provider), ServiceLifetime.Singleton);
        }

        private static IServiceCollection Replace<TService>(this IServiceCollection serviceCollection, Func<IServiceProvider, object> factory, ServiceLifetime serviceLifetime)
        {
            return serviceCollection.Replace(new ServiceDescriptor(typeof(TService), factory, serviceLifetime));
        }

        private static IServiceCollection Replace<TService, TImplementation>(this IServiceCollection serviceCollection, ServiceLifetime serviceLifetime)
        {
            return serviceCollection.Replace(new ServiceDescriptor(typeof(TService), typeof(TImplementation), serviceLifetime));
        }
    }
}
