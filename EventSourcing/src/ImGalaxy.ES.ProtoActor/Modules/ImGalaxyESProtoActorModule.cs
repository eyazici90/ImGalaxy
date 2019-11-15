using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.ProtoActor.Modules
{
    public static class ImGalaxyESProtoActorModule
    {
        public static IServiceCollection AddImGalaxyESProtoActorModule(this IServiceCollection services) =>
            services.RegisterActorManager();
         
        private static IServiceCollection RegisterActorManager(this IServiceCollection services) =>
             services.AddSingleton<IActorManager, ActorManager>();
    }
}
