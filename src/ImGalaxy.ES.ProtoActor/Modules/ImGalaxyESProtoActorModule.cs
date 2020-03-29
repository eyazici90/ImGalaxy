using Microsoft.Extensions.DependencyInjection; 

namespace ImGalaxy.ES.ProtoActor.Modules
{
    public static class ImGalaxyESProtoActorModule
    {
        public static IServiceCollection AddImGalaxyESProtoActorModule(this IServiceCollection services) =>
            services.RegisterActorManager();
         
        private static IServiceCollection RegisterActorManager(this IServiceCollection services) =>
             services.AddSingleton<IActorManager, ActorManager>()
                     .AddSingleton<IActorRouter, ActorManager>();
    }
}
