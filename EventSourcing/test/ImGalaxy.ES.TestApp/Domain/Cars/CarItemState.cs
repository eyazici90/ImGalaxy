using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp.Domain.Cars
{
    public class CarItemState : EntityState<CarItemState>
    {
        public CarItemId _id { get; private set; }
        public CarId _carId { get; private set; }
        public string _desciption { get; private set; }

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
                state._carId = new CarId(@event.CarId);
                state._desciption = @event.Desciption;
            }); 
    }
}
