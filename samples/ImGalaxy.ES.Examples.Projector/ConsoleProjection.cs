using ImGalaxy.ES.Projector;
using System;

namespace ImGalaxy.ES.Examples.Projector
{ 
    public class ConsoleProjection : Projection<ConsoleConnector>
    {
        public ConsoleProjection()
        {
            When<FakeEvent>(async (@event, connector) =>
            {
                connector.Write(@event.Name);
            });
        } 
    }

    public class FakeEvent
    {
        public string Name { get; }
        public FakeEvent(string name)
        {
            Name = name;
        }
    }

    public class ConsoleConnector
    {
        public void Write(string message) =>
            Console.WriteLine(message);
    }
}
