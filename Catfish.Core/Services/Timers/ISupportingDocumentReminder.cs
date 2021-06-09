using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catfish.Core.Services.Timers
{
    public interface ISupportingDocumentReminder
    {
        public Task CheckDocumentReceipt(Guid parentItemId, Guid emailTemplateId, Guid supportingDocTemplateId, string name, string senderEmail, DateTime deadline, PerformContext context);
        public void HangfireTest();
    }
}
