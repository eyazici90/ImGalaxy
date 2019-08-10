using ImGalaxy.ES.Core;
using ImGalaxy.ES.CosmosDB.Modules;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TestCosmos.Application.Commands;
using TestCosmos.Application.Commands.Handlers;

namespace TestCosmos
{
    class Program
    {
        static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();       

        static async Task MainAsync(string[] args)
        {
            var services = new ServiceCollection();

            services
                .AddMediatR(typeof(CreateCarCommandHandler).Assembly)
                .AddGalaxyESCosmosDBModule(configs =>
                {
                    configs.DatabaseId = "TestCosmosES";
                    configs.EventCollectionName = "Events";
                    configs.StreamCollectionName = "Streams";
                    configs.EndpointUri = "https://localhost:8081";
                    configs.PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
                    configs.ReadBatchSize = 100;
                });


            var provider = services.BuildServiceProvider();

            //await AddNewCar(provider);
            await AddItem(provider, "01f157f5-cad5-40b4-9163-2bc4987d3ae2");
            //await ChangeCarName(provider, "642acdb1-38d2-405d-afee-b3a122642cb0");

            //await ChangeModelYear(provider, 2014, "01f157f5-cad5-40b4-9163-2bc4987d3ae2");
            //Exception Case for the last state
            // await ChangeModelYear(provider, 1990, "642acdb1-38d2-405d-afee-b3a122642cb0");

            Console.WriteLine("Done!!!");

            Console.ReadLine();
        }
        private static async Task ChangeCarName(IServiceProvider provider, string id)
        {
            var mediatR = provider.GetRequiredService<IMediator>(); 

            await mediatR.Send(new ChangeCarNameCommand(id, "Mercedes"));  

        }
        private static async Task ChangeModelYear(IServiceProvider provider, int model, string id)
        {
            var mediatR = provider.GetRequiredService<IMediator>();

            await mediatR.Send(new ChangeModelYearCommand(id, model));  
        }

        private static async Task AddNewCar(IServiceProvider provider)
        {
            var mediatR = provider.GetRequiredService<IMediator>();

            await mediatR.Send(new CreateCarCommand("BMW")); 
        }

        private static async Task AddItem(IServiceProvider provider, string id)
        {
            var mediatR = provider.GetRequiredService<IMediator>();

            await mediatR.Send(new AddItemToCarCommand(id, "Excellent Car!!!")); 

        }
    }
}
