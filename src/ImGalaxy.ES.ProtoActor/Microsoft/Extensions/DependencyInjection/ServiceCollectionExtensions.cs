using ImGalaxy.ES.ProtoActor;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddImGalaxyESProtoActorModule(this IServiceCollection services) =>
            services.RegisterActorManager();
         
        private static IServiceCollection RegisterActorManager(this IServiceCollection services) =>
             services.AddSingleton<IActorManager, ActorManager>()
                     .AddSingleton<IActorRouter, ActorManager>();
    }
}
