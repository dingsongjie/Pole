using Pole.ReliableMessage.Abstraction;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage
{
    class DefaultApplicationBuilderConfigurator : IApplicationBuilderConfigurator
    {
        private readonly List<Action<IApplicationBuilder>> _configs = new List<Action<IApplicationBuilder>>();
        public void Add(Action<IApplicationBuilder> config)
        {
            _configs.Add(config);
        }

        public void Config(IApplicationBuilder applicationBuilder)
        {
            _configs.ForEach(m => {
                m(applicationBuilder);
            });
        }
    }
}
