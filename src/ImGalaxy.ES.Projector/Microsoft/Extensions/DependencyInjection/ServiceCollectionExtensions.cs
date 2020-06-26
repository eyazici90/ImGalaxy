using ImGalaxy.ES.Projector;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjector<T>(this IServiceCollection services,
            params Type[] projectionTypes)
            where T : class =>
            AddProjector<T>(services, ServiceLifetime.Singleton, projectionTypes);

        public static IServiceCollection AddProjector<T>(this IServiceCollection services,
            ServiceLifetime serviceLifetime,
            params Type[] projectionTypes)
            where T : class
        {
            services.AddTransient<ServiceFactory>(p => p.GetServices);

            var projectorDescriptor = new ServiceDescriptor(typeof(IProjector), typeof(ConnectedProjector<T>), serviceLifetime);

            services.Add(projectorDescriptor);

            foreach (var projectionType in projectionTypes)
            {
                if (!typeof(IProjection<T>).IsAssignableFrom(projectionType))
                    throw new Exception($"{projectionType.Name} cannot be assigned from {typeof(IProjection<T>).Name}");

                var projectionDescriptor = new ServiceDescriptor(typeof(IProjection<T>), projectionType, ServiceLifetime.Transient);
                services.Add(projectionDescriptor);
            }

            return services;
        }
    }
}
