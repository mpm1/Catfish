using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Catfish.Core.Models.Contents.Workflow
{
    public class Reminder : XmlModel
    {
        public static readonly string TagName = "reminder";

        public static readonly string NameAtt = "name";

        public static readonly string PeriodAtt = "period";

        public static readonly string RepeatAtt = "repeat";
        
        public static readonly string InheritAtt = "inherit";

        public XmlModelList<AdditionalRecipient> AdditionalRecipients { get; set; }

        public string Name
        {
            get => Data.Attribute(NameAtt).Value;
            set => Data.SetAttributeValue(NameAtt, value);
        }
        public string Period
        {
            get => Data.Attribute(PeriodAtt).Value;
            set => Data.SetAttributeValue(PeriodAtt, value);
        }
        public bool Repeat
        {
            get => GetAttribute(RepeatAtt, false);
            set => Data.SetAttributeValue(RepeatAtt, value);
        }
        public bool InheritRecipients
        {
            get => GetAttribute(InheritAtt, false);
            set => Data.SetAttributeValue(InheritAtt, value);
        }
        public override void Initialize(eGuidOption guidOption)
        {
            base.Initialize(guidOption);

            //Initializing the state mappings list
            XElement additionalRecipientDefinition = GetElement("additional-recipients", true);
            AdditionalRecipients = new XmlModelList<AdditionalRecipient>(additionalRecipientDefinition, true, "additional-recipient");
        }
        public EmailRecipient AddRecipientByEmail(string email)
        {
            if (AdditionalRecipients.FindByAttribute(EmailRecipient.EmailAtt, email) != null)
                throw new Exception(string.Format("Email recipient {0} already exists.", email));

            AdditionalRecipient newRecipient = new AdditionalRecipient() { Email = email };
            AdditionalRecipients.Add(newRecipient);
            return newRecipient;
        }

        public EmailRecipient AddRecipientByRole(Guid roleId, string roleName)
        {
            if (AdditionalRecipients.Where(x => x.RoleId == roleId).Any())
                throw new Exception(string.Format("Email recipient role {0} already exists.", roleName));

            AdditionalRecipient newRecipient = new AdditionalRecipient() { RoleId = roleId };
            AdditionalRecipients.Add(newRecipient);
            return newRecipient;
        }

        public EmailRecipient AddRecipientByDataField(Guid dataItemId, Guid fieldId)
        {
            if (AdditionalRecipients.Where(r => r.DataContainerId == dataItemId && r.FieldId == fieldId).Any())
                throw new Exception(string.Format("Email recipient DataItem {0} and Feild {1} already exists.", dataItemId, fieldId));

            AdditionalRecipient newRecipient = new AdditionalRecipient() { DataContainerId = dataItemId, FieldId = fieldId };
            AdditionalRecipients.Add(newRecipient);
            return newRecipient;

        }

        public void AddRecipientByMetadataField(string metadataSetName, Guid fieldId)
        {

        }

        public EmailRecipient AddOwnerAsRecipient()
        {
            if (AdditionalRecipients.Where(x => x.Owner).Any())
                throw new Exception(string.Format("Owner is already a recipient."));

            AdditionalRecipient newRecipient = new AdditionalRecipient() { Owner = true };
            AdditionalRecipients.Add(newRecipient);
            return newRecipient;
        }
        public Reminder(XElement data)
            : base(data)
        {

        }

        public Reminder()
            : base(new XElement(TagName))
        {

        }
    }
}
