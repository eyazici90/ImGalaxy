﻿using EventStore.ClientAPI;
using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.EventStore
{
    public class SubscriptionManager
    {
        private readonly ICheckpointStore _checkpointStore;
        private readonly IEventStoreConnection _connection; 
        private readonly ProjectionHandler[] _projections;  
        private readonly ISnapshotStore[] _snapshotters;
        private readonly IEventDeserializer _eventDeserializer;
        private readonly IEventStoreConfigurator _configurator;

        internal SubscriptionManager(IEventStoreConnection connection, 
            ICheckpointStore checkpointStore,
            ProjectionHandler[] projections,
            ISnapshotStore[] snapshotters,
            IEventDeserializer eventDeserializer,
            IEventStoreConfigurator configurator)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _configurator = configurator ?? throw new ArgumentNullException(nameof(configurator));
            _projections = projections ?? throw new ArgumentNullException(nameof(projections)); 
            _snapshotters = snapshotters ?? throw new ArgumentNullException(nameof(snapshotters)); 
            _checkpointStore = checkpointStore ?? throw new ArgumentNullException(nameof(checkpointStore)); 
            _eventDeserializer = eventDeserializer ?? throw new ArgumentNullException(nameof(eventDeserializer));
        }
        public Task Activate() => Task.WhenAll(_projections.Select(x => StartProjection(x)));

        private async Task StartProjection(ProjectionHandler projection)
        {
            var projectionTypeName = projection.GetType().FullName;

            Position lastCheckpoint = await _checkpointStore.GetLastCheckpoint<Position>(projectionTypeName);

            var settings = new CatchUpSubscriptionSettings(
                this._configurator.MaxLiveQueueSize,
                this._configurator.ReadBatchSize,
                false,
                false,
                projectionTypeName
            );

            _connection.SubscribeToAllFrom(
                lastCheckpoint,
                settings,
                EventAppeared(projection, projectionTypeName),
                LiveProcessingStarted(projection, projectionTypeName),
                SubscriptionDropped(projection, projectionTypeName)
                );
        }

        private Func<EventStoreCatchUpSubscription, ResolvedEvent, Task> EventAppeared(
            ProjectionHandler projection,
            string projectionName
        ) => async (_, e) =>
        {
            // check system event
            if (e.OriginalEvent.EventType.StartsWith("$")) { return; }

            var @event = this._eventDeserializer.Deserialize(Type.GetType(e.Event.EventType), Encoding.UTF8.GetString(e.Event.Data));

            if (@event == null) { throw new ArgumentNullException(nameof(@event)); }

            await projection.Handle(@event);

            await _checkpointStore.SetLastCheckpoint(projectionName, e.OriginalPosition);

            var metadata = this._eventDeserializer.Deserialize<EventMetadata>(Encoding.UTF8.GetString(e.Event.Metadata));

            ISnapshotStore snapshotStore = _snapshotters.FirstOrDefault(
                            x => x.ShouldTakeSnapshot(Type.GetType(metadata.AggregateAssemblyQualifiedName), e) && !metadata.IsSnapshot);

            if (snapshotStore != null)
            {
                await snapshotStore.TakeSnapshot(e.OriginalStreamId);
            }
        };

        private Action<EventStoreCatchUpSubscription, SubscriptionDropReason, Exception> SubscriptionDropped(ProjectionHandler projection, string projectionName)
            => (subscription, reason, ex) =>
            {
                subscription.Stop();

                switch (reason)
                {
                    case SubscriptionDropReason.UserInitiated:
                        break;
                    case SubscriptionDropReason.SubscribingError:
                    case SubscriptionDropReason.ServerError:
                    case SubscriptionDropReason.ConnectionClosed:
                    case SubscriptionDropReason.CatchUpError:
                    case SubscriptionDropReason.ProcessingQueueOverflow:
                    case SubscriptionDropReason.EventHandlerException:
                        throw new Exception($"EventHandler exception occured!!!");
                        //Task.Run(() => StartProjection(projection)); 
                    default:
                        break;
                }
            };

        private static Action<EventStoreCatchUpSubscription> LiveProcessingStarted(ProjectionHandler projection, string projectionName)
            => _ =>
            {

            };
    }
}
