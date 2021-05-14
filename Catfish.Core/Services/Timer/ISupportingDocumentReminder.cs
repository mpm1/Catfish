using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catfish.Core.Services.Timer
{
    public interface ISupportingDocumentReminder
    {
        public Task CheckDocumentReceipt(Guid parentItemId, Guid supportingDocTemplateId, string senderEmail, DateTime deadline);
    }
}
