using Catfish.Core.Services.Timers;
using Hangfire;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catfish.UnitTests
{
    public class TimerTest
    {

        [Test]
        public void SupportingDocumentReceiveCheck()
        {
            //This routine schedules a background job to check whether a supporting document
            //requested for a particular item from a specific reviewer has been received by 
            //the specified deadline, and if not, triggers an email to a specified recipient

            //Paramters 
            Guid itemId = Guid.Parse("6D07649B-125F-47B8-A142-387A4C9ADE34");
            Guid supportingDocTemplateId = Guid.Parse("c234bc7d-891e-4b9c-94c2-861f3496a171");
            string reviewerEmail = "iwickram@ualberta.ca";
            string name = "daily email reminder";
            DateTime deadline = DateTime.Now.AddSeconds(5);
            BackgroundJob.Schedule<ISupportingDocumentReminder>(
                x => x.CheckDocumentReceipt(itemId, supportingDocTemplateId,name,  reviewerEmail, deadline),
                deadline.Subtract(DateTime.Now));
        }
    }
}
