using Catfish.Core.Models;
using Catfish.Core.Models.Contents;
using Catfish.Services;
using ElmahCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catfish.Core.Services.Timers
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
        public void CheckDocumentReceipt(Guid parentItemId, Guid supportingDocTemplateId, string name, string senderEmail, DateTime deadline)
        {
            var item = _db.Items.Where(i => i.Id == parentItemId).FirstOrDefault();
            item.AddTimer(name, supportingDocTemplateId, DateTime.Now, deadline, senderEmail, false);
            _db.SaveChanges();
            //return item;
        }

        public void HangfireTest()
        {
            string filename = "c:\\HangfireTest.txt";
            System.IO.File.WriteAllText(filename, DateTime.Now.ToString());
            System.IO.File.AppendAllText(filename, _db == null ? "DB Null" : "DB NOT NULL");
        }
    }
}
