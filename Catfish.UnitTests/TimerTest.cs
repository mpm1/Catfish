using Catfish.Core.Models;
using Catfish.Core.Services.Timers;
using Catfish.Test.Helpers;
using Hangfire;
using Hangfire.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catfish.UnitTests
{
    public class TimerTest
    {
        private protected AppDbContext _db;
        private protected TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = new TestHelper();
            _db = _testHelper.Db;
        }

        [Test]
        public void SupportingDocumentReceiveCheck()
        {
            //This routine schedules a background job to check whether a supporting document
            //requested for a particular item from a specific reviewer has been received by 
            //the specified deadline, and if not, triggers an email to a specified recipient

            //Paramters 
            Guid itemId = Guid.Parse("6D07649B-125F-47B8-A142-387A4C9ADE34");
            Guid supportingDocTemplateId = Guid.Parse("111e2c81-70a9-4483-81f0-8e89ed2bf07a");
            Guid EmailTemplate = Guid.Parse("b2547544-c22a-456e-9133-0bc6636c6ad3");
            string reviewerEmail = "iwickram@ualberta.ca";
            string name = "daily email reminder";
            DateTime deadline = DateTime.Now.AddSeconds(5);
            var offset = DateTimeOffset.Now.AddSeconds(5);
            //BackgroundJob.Schedule<SupportingDocumentReminder>(
            //    x => x.CheckDocumentReceipt(itemId, EmailTemplate, supportingDocTemplateId, name,  reviewerEmail, deadline, null),
            //    offset);
            RecurringJob.AddOrUpdate<SupportingDocumentReminder>(
                x => x.CheckDocumentReceipt(itemId, EmailTemplate, supportingDocTemplateId, name, reviewerEmail, deadline, null),
                Cron.Minutely);

        }

        [Test]
        public void HangfireTest()
        {
            var offset = DateTimeOffset.Now.AddSeconds(5);
            //BackgroundJob.Schedule<ISupportingDocumentReminder>(x => x.HangfireTest(), offset);
            RecurringJob.AddOrUpdate<SupportingDocumentReminder>(x => x.HangfireTest(), Cron.Minutely);

        }
    }
}
