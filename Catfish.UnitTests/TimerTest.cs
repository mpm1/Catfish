using Catfish.Core.Services.Timer;
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
            Guid itemId = Guid.NewGuid();
            Guid supportingDocTemplateId = Guid.NewGuid();
            string reviewerEmail = "iwickram@ualberta.ca";
            DateTime deadline = DateTime.Now.AddSeconds(5);
            BackgroundJob.Schedule<ISupportingDocumentReminder>(
                x => x.CheckDocumentReceipt(itemId, supportingDocTemplateId, reviewerEmail, deadline),
                deadline.Subtract(DateTime.Now));
        }
    }
}
