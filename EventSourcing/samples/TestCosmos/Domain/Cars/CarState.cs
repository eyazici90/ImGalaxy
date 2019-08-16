using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestCosmos.Domain.Cars;
using TestCosmos.Domain.Cars.Snapshots;

namespace TestCosmos
{
    public class CarState : AggregateRootState<CarState> , ISnapshotable
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

            _carItems = _carItems ?? new List<CarItemState>();
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

        public void RestoreSnapshot(object state)
        {
            var snapshot = (CarStateSnapshot)state;
            this._id = new CarId(snapshot.Id);
            this._name = snapshot.Name;
            this._year = snapshot.Year;

            this._carItems = snapshot.CarItems.Select(c => CarItem.Create(c.Id, new CarId(c.CarId), c.Desciption).State).ToList() ?? this._carItems;
        }

        public object TakeSnapshot() =>
            new CarStateSnapshot
            {
                Id = this._id,
                Name = this._name,
                Year = this._year,
                CarItems = this._carItems.Select(c=>new CarItemStateSnapshot { CarId = c._carId, Id = c._id, Desciption = c._desciption})
                                    .ToList()
            };

    }
}
