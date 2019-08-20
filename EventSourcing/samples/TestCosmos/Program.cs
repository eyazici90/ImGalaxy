using ImGalaxy.ES.Core;
using ImGalaxy.ES.CosmosDB;
using ImGalaxy.ES.CosmosDB.Documents;
using ImGalaxy.ES.CosmosDB.Modules; 
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TestCosmos.Application.Commands;
using TestCosmos.Application.Commands.Handlers;
using TestCosmos.Domain.Cars.Snapshots;

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
                .AddImGalaxyESCosmosDBModule(configs =>
                {
                    configs.DatabaseId = "TestCosmosES";
                    configs.EventCollectionName = "Events";
                    configs.StreamCollectionName = "Streams";
                    configs.SnapshotCollectionName = "Snapshots";
                    configs.EndpointUri = "https://localhost:8081";
                    configs.PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
                    configs.ReadBatchSize = 100;
                    configs.OfferThroughput = 400;
                    configs.IsSnapshottingOn = true;
                    configs.SnapshotStrategy = @event => @event.Position %2 == 0;

                }); 

            var provider = services.BuildServiceProvider();

            await provider.UseGalaxyESCosmosDBModule();


            var mediatR = provider.GetRequiredService<IMediator>();
            var configurations = provider.GetRequiredService<ICosmosDBConfigurations>();
            var cosmosClient = provider.GetRequiredService<ICosmosDBClient>();
            var rootRepo = provider.GetRequiredService<IAggregateRootRepository<CarState>>();
            var unitofWork = provider.GetRequiredService<IUnitOfWork>();
            var serializer = provider.GetRequiredService<IEventSerializer>();


            //  await AddNewCar(provider, mediatR);
            //await AddItem(provider, mediatR, "f8377871-a09f-4b01-9edb-47d44762f6f6"); 
            // await ChangeCarName(provider, mediatR, "f8377871-a09f-4b01-9edb-47d44762f6f6");


            // await ChangeModelYear(provider, mediatR, 2014, "b7887eba-896c-4f9d-9d56-7156de817b8d");
            //Exception Case for the last state

            //await ChangeModelYear(provider, mediatR, 1990, "642acdb1-38d2-405d-afee-b3a122642cb0");

            //for (int i = 0; i < 100; i++)
            //{
            //    await Task.Factory.StartNew(()=> {
            //        ChangeCarName(provider, mediatR, "97d264eb-adfd-45ad-ace6-01408b2b7bc6").GetAwaiter().GetResult(); 
            //    });
            //}

            //var snapshotter = new SnapshotterCosmosDB<CarState, CarStateSnapshot>(rootRepo, unitofWork, cosmosClient, configurations,
            //  serializer);

            //var eDocs = cosmosClient.GetDocumentQuery<EventDocument>(e => e.StreamId == "f8377871-a09f-4b01-9edb-47d44762f6f6", "Events")
            //   .OrderBy(e => e.Position)
            //   .ToList();

            //foreach (var e in eDocs)
            //{
            //    var shouldWeTakeSnapshot = snapshotter.ShouldTakeSnapshot(typeof(CarState), e);
            //    if (shouldWeTakeSnapshot)
            //        await snapshotter.TakeSnapshot("f8377871-a09f-4b01-9edb-47d44762f6f6");

            //}


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

        private static async Task TakeSnapshots()
        {

        }
     
    }
}
