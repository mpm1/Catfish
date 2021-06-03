//CREDITS: https://github.com/gonzigonz/HangfireCore-Example/blob/master/HangfireCore.Mvc/Infrastructure/HangfireActivator.cs

using System;
using Hangfire;

namespace Catfish.Core.Infrastructure
{
    public class HangfireActivator : JobActivator
    {
        private readonly IServiceProvider _serviceProvider;

        public HangfireActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override object ActivateJob(Type type)
        {
            return _serviceProvider.GetService(type);
        }
    }
}
