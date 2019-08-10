using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;
using TestCosmos.Domain.Cars;

namespace TestCosmos
{
    public class CarState : AggregateRootState<CarState> , IAggregateRoot
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
}
