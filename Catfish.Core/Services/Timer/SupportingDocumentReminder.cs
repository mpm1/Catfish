using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catfish.Core.Services.Timer
{
    public class SupportingDocumentReminder : ISupportingDocumentReminder
    {
        public Task CheckDocumentReceipt(Guid parentItemId, Guid supportingDocTemplateId, string senderEmail, DateTime deadline)
        {
            throw new NotImplementedException();
        }
    }
}
