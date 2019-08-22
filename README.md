
# ImGalaxy
C# library for CQRS, EventSourcing needs with various provider supports. (EventStore, Azure Cosmos DB)


##  Features 
 -   Support for EventStore ([https://github.com/EventStore/EventStore](https://github.com/EventStore/EventStore))
 -   Support for Azure Cosmos DB
 - Stream based persistence & querying
 - Optimistic concurrency for event persistence
 - Event first functional aggregate modelling
 
## Upcoming Features
 - InMemory provider support for behaviour testing

## Composition Root
***for Azure CosmosDB provider***

    services
	    .AddImGalaxyESCosmosDBModule(configs =>
                {
                    configs.DatabaseId = "<your_database_id>";
                    configs.EventCollectionName = "<event_collection_name>";
                    configs.StreamCollectionName = "<stream_collection_name>";
                    configs.SnapshotCollectionName = "<snapshot_collection_name>";
                    configs.EndpointUri = "<your_endpoint>";
                    configs.PrimaryKey = "<your_primary>";
                    configs.ReadBatchSize = <batch_size>;
                    configs.OfferThroughput = <offer_throughput>;
                }); 

***for EventStore provider***

    services
	    .AddImGalaxyESEventStoreModule(configs=> 
                {
                    configs.Uri = "<your_eventstore_uri>";
                    configs.Username = "<your_user_name>";
                    configs.Password = "<your_pass>";
                    configs.ReadBatchSize =   <batch_size>;
                    configs.MaxLiveQueueSize = <your_qlive_size>
                }); 

## Usages

*Aggregate Root State Ex.*

     public class CarState : AggregateRootState<CarState> 
    {
        public CarId _id { get; private set; }
        public string _name { get; private set; }
        public int _year { get; private set; } 
        public List<CarItemState> _carItems { get; private set; }
        public IReadOnlyCollection<CarItemState> CarItems => _carItems.AsReadOnly();

        private CarState()
        {
            RegisterEvent<CarRegisteredEvent>(When);
            RegisterEvent<CarNameChangedEvent>(When);
            RegisterEvent<CarModelRenewedEvent>(When);
            RegisterEvent<CarItemAddedEvent>(When); 
        }

        internal CarState(CarId id) : this()
        {
            id.ThrowsIfNull(new ArgumentNullException(id));
        }

        private void When(CarModelRenewedEvent @event) =>
            With(this, state => 
            {
                _year = @event.Year;
            });

        private void When(CarNameChangedEvent @event) =>
            With(this, state=> 
            {
                _name = @event.Name;
            });

        private void When(CarRegisteredEvent @event) =>
            With(this, state=>
            {
                state._id = new CarId(@event.Id);
                state._name = @event.Name;
            });

        private void When(CarItemAddedEvent @event) =>
            With(this, state =>
            {
                var newItem = CarItem.Create(new CarItemId(@event.CarItemId), @event.CarId, @event.Desciption);

                state._carItems = state._carItems ?? new List<CarItemState>();

                state._carItems.Add(newItem.State);

            });
    }
    
*Aggregate Root Behaviour Ex.*
 

     public static class Car
    {
        public static CarState.Result RegisterCar(string id, string name) =>
         new CarState(new CarId(id)).ApplyEvent(new CarRegisteredEvent(id, name));

        public static CarState.Result ChangeName(CarState state, string name) =>
            state.ApplyEvent(new CarNameChangedEvent(state._id, name));

        public static CarState.Result RenewModel(CarState state, int year) =>
            state.ThrowsIf(c => c._year > year, new Exception("model cannot be above than 2019"))
                .ApplyEvent(new CarModelRenewedEvent(state._id, year));

        public static CarState.Result AddCarItem(CarState state, CarItemId carItemId, string desc)=>
            state.ThrowsIf(s=>s.CarItems.Count == 2, new Exception("You cannot add more than 2 items to single car"))
                 .ApplyEvent(new CarItemAddedEvent(state._id, carItemId, desc));
    }
