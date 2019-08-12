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
            //    .AddGalaxyESEventStoreModule(configs =>
            //    {
            //        configs.Username = "admin";
            //        configs.Password = "changeit";
            //        configs.Uri = "tcp://admin:changeit@localhost:1113";
            //        configs.ReadBatchSize = 1000;
            //        configs.MaxLiveQueueSize = 100;
            //    });

            var provider = services.BuildServiceProvider();
            var mediatR = provider.GetRequiredService<IMediator>();

          //  await AddNewCar(provider, mediatR);
            //await AddItem(provider, mediatR, "01f157f5-cad5-40b4-9163-2bc4987d3ae2");

              await ChangeCarName(provider, mediatR, "39566137-83c6-4dd7-a6ef-01d489d341a2");


            //await ChangeModelYear(provider, mediatR, 2014, "01f157f5-cad5-40b4-9163-2bc4987d3ae2");
            //Exception Case for the last state
            //await ChangeModelYear(provider, mediatR, 1990, "642acdb1-38d2-405d-afee-b3a122642cb0");

            Console.WriteLine("Done!!!");

            Console.ReadLine();
        }
        private static async Task ChangeCarName(IServiceProvider provider, IMediator mediatR, string id)=>   
            await mediatR.Send(new ChangeCarNameCommand(id, "Mercedes"));  
         
        private static async Task ChangeModelYear(IServiceProvider provider, IMediator mediatR, int model, string id)=>
            await mediatR.Send(new ChangeModelYearCommand(id, model));  
         
        private static async Task AddNewCar(IServiceProvider provider, IMediator mediatR)=>   
            await mediatR.Send(new CreateCarCommand("BMW")); 
         
        private static async Task AddItem(IServiceProvider provider, IMediator mediatR, string id) => 
            await mediatR.Send(new AddItemToCarCommand(id, "Excellent Car!!!"));  
     
    }
}
