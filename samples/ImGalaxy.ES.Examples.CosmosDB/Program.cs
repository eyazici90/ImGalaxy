using ImGalaxy.ES.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ImGalaxy.ES.Examples.CosmosDB
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var provider = BuildServiceProviderWithCosmosModule();

            var aggregateStore = provider.GetRequiredService<IAggregateStore>();

            var productId = Product.ProductId.New;


            var result = Product.Create(productId, new Product.ProductCode("P012")); 

            await aggregateStore.Save(result.State.ProductId, ExpectedVersion.NoStream, result).ConfigureAwait(false);

            var aggregate = await aggregateStore.Load<Product.State>(productId).ConfigureAwait(false);

            var state = aggregate.Root as Product.State;



            Console.WriteLine($"Product Code : {state.ProductCode}");
            Console.ReadLine();

        }
        static IServiceProvider BuildServiceProviderWithCosmosModule()
        {
            var services = new ServiceCollection();

            services.AddImGalaxyESCosmosDBModule(config =>
            {
                config.DatabaseId = "TestCosmosES";
                config.EndpointUri = "https://localhost:8081";
                config.PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
                config.ReadBatchSize = 1000;
            });

            return services.BuildServiceProvider();
        }
    }
}
