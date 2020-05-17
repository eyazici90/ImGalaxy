using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using static ImGalaxy.ES.Examples.CosmosDB.Product.Events;

namespace ImGalaxy.ES.Examples.CosmosDB
{
    public static class Product
    {
        public class State : AggregateRootState<State>
        {
            public ProductId ProductId { get; private set; }
            public ProductCode ProductCode { get; private set; }
            internal State()
            {
                RegisterEvent<ProductCreated>(When);
            }

            private void When(ProductCreated @event) =>
                With(this, state =>
                {
                    ProductId = new ProductId(@event.ProductId);
                    ProductCode = new ProductCode(@event.Code);
                });
        }
        public static State.Result Create(ProductId productId,
            ProductCode productCode) =>
            new State().ApplyEvent(new ProductCreated(productId, productCode.Value));


        public class Events
        {
            public class ProductCreated
            {
                public string ProductId { get; }
                public string Code { get; }
                public ProductCreated(string productId,
                    string code)
                {
                    ProductId = productId;
                    Code = code;
                }
            }
        }

        public class ProductId : Identity<string>
        {
            public static ProductId New => new ProductId(Guid.NewGuid().ToString());
            public ProductId(string id) : base(id)
            {
            }
            public static implicit operator string(ProductId self) => self.Id;
        }
        public class ProductCode : ValueObject
        {
            public string Value { get; }
            public ProductCode(string value)
            {
                Value = value;
            }

            protected override IEnumerable<object> GetAtomicValues()
            {
                yield return Value;
            }

        }
    }
}
