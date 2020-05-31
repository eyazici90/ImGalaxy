using ImGalaxy.ES.Core;
using ImGalaxy.ES.InMemory;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddImGalaxyESInMemoryModule(this IServiceCollection services) =>
               services.With(s =>
               {
                   s.RegisterProviders()
                    .RegisterChangeTracker()
                    .RegisterInMemoryConnection()
                    .RegisterAggregateStore()
                    .RegisterRepositories() 
                    .RegisterUnitOfWork();
               });
         
        private static IServiceCollection RegisterProviders(this IServiceCollection services) =>
                     services.AddTransient<ISnapshotReader, NullInstanceSnapshotReader>();
        private static IServiceCollection RegisterChangeTracker(this IServiceCollection services) =>
           services.AddScoped<IAggregateChangeTracker, AggregateChangeTracker>();
        private static IServiceCollection RegisterRepositories(this IServiceCollection services) =>
             services.AddScoped(typeof(IAggregateRootRepository<>), typeof(AggregateRootRepository<>));
        private static IServiceCollection RegisterInMemoryConnection(this IServiceCollection services) =>
         services.AddSingleton<IInMemoryConnection, InMemoryConnection>();

        private static IServiceCollection RegisterUnitOfWork(this IServiceCollection services) =>
             services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();

        private static IServiceCollection RegisterAggregateStore(this IServiceCollection services) =>
            services.AddTransient<IAggregateStore, AggregateStore>();
    }
}
