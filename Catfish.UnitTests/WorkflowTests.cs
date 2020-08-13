﻿using Catfish.Core.Models;
using Catfish.Core.Models.Contents.Data;
using Catfish.Core.Models.Contents.Fields;
using Catfish.Core.Models.Contents.Workflow;
using Catfish.Services;
using Catfish.Tests.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catfish.UnitTests
{
    public class WorkdlowTests
    {
        protected AppDbContext _db;
        protected TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = new TestHelper();
            _db = _testHelper.Db;
        }


        [Test]
        public void ContractLetterWorkflowBuildTest()
        {
            string lang = "en";
            EntityTemplate template = new EntityTemplate();
            template.TemplateName = "Trust Funded GRA/GRAF Contract";
            template.Name.SetContent(template.TemplateName);
            
            IWorkflowService ws = _testHelper.WorkflowService;
            ws.SetModel(template);
            
            //Email template for trust account hoder
            EmailTemplate trustAccountHolderNotification = ws.GetEmailTemplate("Trust Account Holder Notification", true);
            trustAccountHolderNotification.SetDescription("This metadata set defines the email template to be sent for the trust-account holder when a new contract is created.", lang);
            trustAccountHolderNotification.SetSubject("Trust-funded Contract Review");
            trustAccountHolderNotification.SetBody("Please review @Link[this contract letter|@Model] to be funded by one of your trust accounts and provide your decision within 2 weeks.\n\nThank you");

            //Email template for the associate chair
            EmailTemplate deptChairNotification = ws.GetEmailTemplate("Associate Chair Notification", true);
            deptChairNotification.SetDescription("This metadata set defines the email template to be sent for the associate chair when a new contract is created.", lang);
            deptChairNotification.SetSubject("Graduate Contract Review (Trust Funded)");
            deptChairNotification.SetBody("Please review @Link[this contract letter|@Model] to be funded by a trust accounts and provide your decision within 2 weeks.\n\nThank you");

            //Email template for the department admin
            EmailTemplate deptAdminNotification = ws.GetEmailTemplate("Department Admin Notification", true);
            deptAdminNotification.SetDescription("This metadata set defines the email template to be sent for the program admin when another party makes a change to a contract.", lang);
            deptAdminNotification.SetSubject("Cnotract Status Update (Trust Funded)");
            deptAdminNotification.SetBody("The status of @Link[this contract letter|@Model] has been updated.\n\nThank you");

            //Contract letter
            DataItem contract = ws.GetDataItem("Contract Letter", true);
            contract.SetDescription("This is the template for the contract letter.", lang);
            contract.CreateField<TextField>("First Name", lang, true);
            contract.CreateField<TextField>("Last Name", lang, true);
            contract.CreateField<TextField>("Student ID", lang, true);
            contract.CreateField<TextField>("Student Email", lang, true, true);
            contract.CreateField<TextField>("Department", lang, true, false, "East Asian Studies");
            contract.CreateField<SelectField>("Type of Appointment", lang, new string[] { "Graduate Research Assistant", "Graduate Research Assistantship Fellowship" }, true, 0);
            contract.CreateField<TextField>("Assignment", lang, true);

            contract.CreateField<InfoSection>("Period of Appointment", lang, "alert alert-info");
            contract.CreateField<DateField>("Appointment Start", lang, true);
            contract.CreateField<DateField>("Appointment End", lang, true);

            contract.CreateField<InfoSection>("Stipend", lang, "alert alert-info");
            contract.CreateField<IntegerField>("Rate", lang, true);
            contract.CreateField<IntegerField>("Award", lang, true);
            contract.CreateField<IntegerField>("Salary", lang, true);

            //Save the template to the database
            AppDbContext db = _testHelper.Db;
            EntityTemplate oldTemplate = db.EntityTemplates.Where(et => et.TemplateName == template.TemplateName).FirstOrDefault();
            if (oldTemplate == null)
                db.EntityTemplates.Add(template);
            else
                oldTemplate.Content = template.Content;
            db.SaveChanges();

            //Save the template to a file
            template.Data.Save("..\\..\\..\\..\\Examples\\ContractLetterWorkflow.xml");
        }

        [Test]
        public void CalendarManagementSystemWorkflowBuildTest()
        {
            string lang = "en";
            EntityTemplate template = new EntityTemplate();
            template.Name.SetContent("Calendar management System Workflow");

            IWorkflowService ws = _testHelper.WorkflowService;
            ws.SetModel(template);

            //Get the Workflow object using the workflow service
            Workflow workflow = ws.GetWorkflow(true);

            //Defininig states
            workflow.AddState("Saved");
            workflow.AddState("Submitted");
            workflow.AddState("With AEC");


            //Defininig roles
            WorkflowRole centralAdminRole = workflow.AddRole("CentralAdmin");
            WorkflowRole departmentAdmin = workflow.AddRole("DepartmentlAdmin");

            //Defining users
            WorkflowUser user = workflow.AddUser("centraladmin@ualberta.ca");
            user.AddRoleReference(centralAdminRole);
            WorkflowUser deptUser = workflow.AddUser("departmentadmin@ualberta.ca");
            deptUser.AddRoleReference(departmentAdmin);

            //Defining triggers
            EmailTrigger emailTrigger = workflow.AddTrigger("ToCentralAdmin", "SendEmail");
            emailTrigger.AddRecipientByEmail(user.Email);
            //emailTrigger.AddTemplate(Guid.Parse("57ed6d6c-5bad-469f-b2ce-638dd0c9e68e"), Guid.Parse("e12fd686-7d89-4610-92e2-601b219e5925"));
            EmailTrigger deptAdminEmailTrigger = workflow.AddTrigger("ToDepartmentAdmin", "SendEmail");
            deptAdminEmailTrigger.AddRecipientByRole("DepartmentlAdmin");

            //Defining actions
            GetAction action = workflow.AddAction("Start Submission", "Create", "Home");
            action.AddTemplate(Guid.Parse("57ed6d6c-5bad-469f-b2ce-638dd0c9e68e"));

            //Defining post actions
            PostAction postActionSave = action.AddPostAction("Save", "Save");
            postActionSave.AddStateMapping("*", "Saved");
            PostAction postActionSubmit = action.AddPostAction("Submit", "Save");
            postActionSubmit.AddStateMapping("*", "Submitted");

            //Defining pop-ups
            PopUp popUp = postActionSubmit.AddPopUp("WARNING: Submitting Document", "Once submitted, you cannot update the document.");
            popUp.AddButtons("Yes, submit", "true");
            popUp.AddButtons("Cancel", "false");

            //Defining trigger refs
            postActionSubmit.AddTriggerRefs("0", emailTrigger.Id);
            postActionSubmit.AddTriggerRefs("1", deptAdminEmailTrigger.Id);


            //Defining authorizatios
            action.AddAuthorization(departmentAdmin.Id);

            template.Data.Save("..\\..\\..\\..\\Examples\\CalendarManagementWorkflow_generared.xml");

        }
    }
}
