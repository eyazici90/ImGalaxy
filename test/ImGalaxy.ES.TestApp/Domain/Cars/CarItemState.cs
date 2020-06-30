using Galaxy.Railway;
using ImGalaxy.ES.Core;
using System; 

namespace TestApp.Domain.Cars
{
    public class CarItemState : EntityState<CarItemState>
    {
        public CarItemId _id { get; private set; }
        public CarId CarId { get; private set; } 
        public string Desciption { get; private set; }

        private CarItemState()
        {
            RegisterEvent<CarItemAddedEvent>(When);
        }
        internal CarItemState(CarItemId id, CarId carId) : this()
        {
            id.ThrowsIfNull(new ArgumentNullException(id));
            carId.ThrowsIfNull(new ArgumentNullException(carId));
        }

        private void When(CarItemAddedEvent @event) =>
            With(this, state=> 
            {
                state._id = new CarItemId(@event.CarItemId);
                state.CarId = new CarId(@event.CarId);
                state.Desciption = @event.Desciption;
            }); 
    }
}
