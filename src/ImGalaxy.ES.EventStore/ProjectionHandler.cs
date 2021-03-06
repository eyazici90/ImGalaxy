﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.EventStore
{
    public abstract class ProjectionHandler
    {
        private readonly Type _type;

        protected ProjectionHandler() => _type = GetType();

        public abstract Task Handle(object e);

        public override string ToString() => _type.Name;

        public static implicit operator string(ProjectionHandler self) => self.ToString();
    }
}
