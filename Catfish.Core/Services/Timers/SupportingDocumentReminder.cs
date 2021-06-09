using Catfish.Core.Helpers;
using Catfish.Core.Models;
using Catfish.Core.Models.Contents;
using Catfish.Core.Models.Contents.Workflow;
using Catfish.Services;
using ElmahCore;
using Hangfire;
using Hangfire.Server;
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
        private readonly IEmailService _emailService;
        public SupportingDocumentReminder(AppDbContext db, ErrorLog errorLog, IEmailService emailService)
        {
            _db = db;
            _errorLog = errorLog;
            _emailService = emailService;
        }
        public Task CheckDocumentReceipt(Guid parentItemId, Guid emailTemplateId, Guid supportingDocTemplateId, string name, string senderEmail, DateTime deadline, PerformContext context)
        {
            string jobId = context.BackgroundJob.Id;
            var item = _db.Items.Where(i => i.Id == parentItemId).FirstOrDefault();
            EntityTemplate entityTemplate = _db.EntityTemplates.Where(et => et.Id == item.TemplateId).FirstOrDefault();
            var timers = item.Timers.ToList();
            string lang = "en";
            bool isChildFormSubmitted = item.DataContainer.Where(dc => dc.TemplateId== supportingDocTemplateId).Any();
            foreach (var timer in timers)
            {
                if(timer.Deadline > DateTime.Now && isChildFormSubmitted == false)
                {
                    //get email template name from metadate set.
                    var emailTemplateName = entityTemplate.MetadataSets
                                            .Where(ms => ms.Id == emailTemplateId)
                                            .FirstOrDefault().Name.Values
                                            .Select(ms => ms.Value)
                                            .FirstOrDefault();

                    //get email template using workflow service GetEmailTemplate
                    EmailTemplate emailMessage = entityTemplate.GetEmailTemplate(emailTemplateName, lang, false);
                    if (emailMessage != null)
                    {
                        //Make a clone and update references in the email body
                        emailMessage = emailMessage.Clone<EmailTemplate>();
                        emailMessage.UpdateRerefences("@SiteUrl", ConfigHelper.SiteUrl.TrimEnd('/'));
                        emailMessage.UpdateRerefences("@Item.Id", item.Id.ToString());
                        emailMessage.SetSubject("Reminder:" + name);
                    }
                    
                    Email email = new Email();
                    email.UserName = item.UserEmail;
                    email.Subject = emailMessage.GetSubject();
                    email.FromEmail = ConfigHelper.StmpEmail;
                    email.RecipientEmail = senderEmail;
                    email.Body = emailMessage.GetBody();
                    _emailService.SendEmail(email);
                }
                else
                {
                    RecurringJob.RemoveIfExists(jobId);
                }
            }
            //item.AddTimer(name, supportingDocTemplateId, DateTime.Now, deadline, senderEmail, false);
            _db.SaveChanges();
            return Task.CompletedTask;
        }

        public void HangfireTest()
        {
            string filename = "c:\\Test\\HangfireTest.txt";
            System.IO.File.WriteAllText(filename, DateTime.Now.ToString());
            System.IO.File.AppendAllText(filename, _db == null ? "DB Null" : "DB NOT NULL");
        }
    }
}
