using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catfish.Core.Services.Timers
{
    public interface ISupportingDocumentReminder
    {
        public void CheckDocumentReceipt(Guid parentItemId, Guid emailTemplateId, Guid supportingDocTemplateId, string name, string senderEmail, DateTime deadline);
        public void HangfireTest();
    }
}
