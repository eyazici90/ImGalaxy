using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestApp.Domain.Cars;
using TestApp.Domain.Cars.Snapshots;

namespace TestApp
{
    public class CarState : AggregateRootState<CarState> , ISnapshotable
    {
        public CarId Id { get; private set; }

        private string _name;

        public int Year { get; private set; }

        private List<CarItemState> _carItems;
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
                Year = @event.Year;
            });

        private void When(CarNameChangedEvent @event) =>
            With(this, state=> 
            {
                _name = @event.Name;
            });

        private void When(CarRegisteredEvent @event) =>
            With(this, state=>
            {
                state.Id = new CarId(@event.Id);
                state._name = @event.Name;
            });

        private void When(CarItemAddedEvent @event) =>
            With(this, state =>
            {
                var newItem = CarItem.Create(new CarItemId(@event.CarItemId), @event.CarId, @event.Desciption);

                state._carItems = state._carItems ?? new List<CarItemState>();

                state._carItems.Add(newItem.State);

            });

        public void RestoreSnapshot(object state)
        {
            var snapshot = (CarStateSnapshot)state;
            this.Id = new CarId(snapshot.Id);
            this._name = snapshot.Name;
            this.Year = snapshot.Year;
            this._carItems = this._carItems ?? new List<CarItemState>();
            this._carItems = snapshot.CarItems?.Select(c => CarItem.Create(c.Id, new CarId(c.CarId), c.Desciption).State).ToList() ?? this._carItems;
        }

        public object TakeSnapshot() =>
            new CarStateSnapshot
            {
                Id = this.Id,
                Name = this._name,
                Year = this.Year,
                CarItems = this._carItems?.Select(c=>new CarItemStateSnapshot { CarId = c.CarId, Id = c._id, Desciption = c.Desciption})
                                    .ToList()
            };

    }
}
