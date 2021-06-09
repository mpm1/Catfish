using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catfish.Core.Services.Timers
{
    public class GetJobId : JobFilterAttribute//, IClientFilter, IServerFilter, IElectStateFilter, IApplyStateFilter
    {
        [ThreadStatic]
        private static string _jobId;
        public static string JobId { get { return _jobId; } set { _jobId = value; } }
        public void OnPerforming(PerformingContext filterContext)
        {
            _jobId = filterContext.BackgroundJob.Id;
        }
    }
}
