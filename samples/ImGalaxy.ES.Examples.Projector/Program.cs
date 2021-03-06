﻿using ImGalaxy.ES.Projector;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImGalaxy.ES.Examples.Projector
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var connector = new ConsoleConnector();

            var projector = new ConnectedProjector<ConsoleConnector>(connector, _ => new List<IProjection<ConsoleConnector>>
            {
              new ConsoleProjection()
            });

            var @event = new FakeEvent("Amsterdam");

            await projector.ProjectAsync(@event).ConfigureAwait(false);
        }
    }
}
