using Catfish.Core.Models;
using Catfish.Services;
using ElmahCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catfish.Core.Services.Timer
{
    public class SupportingDocumentReminder : ISupportingDocumentReminder
    {
        private readonly AppDbContext _db;
        private readonly ErrorLog _errorLog;
        public SupportingDocumentReminder(AppDbContext db, ErrorLog errorLog)
        {
            _db = db;
            _errorLog = errorLog;
        }
        public Task CheckDocumentReceipt(Guid parentItemId, Guid supportingDocTemplateId, string senderEmail, DateTime deadline)
        {
            var item = _db.Items.Where(i => i.Id == parentItemId).FirstOrDefault();
            var entityTemplate = _db.EntityTemplates.Where(it => it.TemplateId == item.TemplateId).FirstOrDefault();

            throw new NotImplementedException();
        }
    }
}
