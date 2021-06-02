using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Catfish.Core.Models.Contents
{
    public class Timer : XmlModel
    {
        public static readonly string TagName = "timer";

        public static readonly string NameAtt = "name";

        public static readonly string ChildTemplateAtt = "child-template-id";

        public static readonly string RequestDateAtt = "request-date";

        public static readonly string DeadlineAtt = "deadline";

        public static readonly string DocOwnerAtt = "supporting-doc-owner";

        public static readonly string ExecuteAtt = "executed";

        public string Name
        {
            get => Data.Attribute(NameAtt).Value;
            set => Data.SetAttributeValue(NameAtt, value);
        }
        public Guid? ChildTemplateId
        {
            get => GetAttribute(ChildTemplateAtt, null as Guid?);
            set => Data.SetAttributeValue(ChildTemplateAtt, value);
        }
        public DateTime RequestDate
        {
            get => GetDateTimeAttribute(RequestDateAtt).Value;
            set => Data.SetAttributeValue(RequestDateAtt, value);
        }
        public DateTime Deadline
        {
            get => GetDateTimeAttribute(DeadlineAtt).Value;
            set => Data.SetAttributeValue(DeadlineAtt, value);
        }
        public string DocumentOwner
        {
            get => Data.Attribute(DocOwnerAtt).Value;
            set => Data.SetAttributeValue(DocOwnerAtt, value);
        }
        public bool Executed
        {
            get => GetAttribute(ExecuteAtt, false);
            set => Data.SetAttributeValue(ExecuteAtt, value);
        }

        public Timer(XElement data)
            : base(data)
        {

        }

        public Timer()
            : base(new XElement(TagName))
        {

        }
        public DateTime? GetDateTimeAttribute(string key)
        {
            var att = Data.Attribute(key);
            return (att == null || string.IsNullOrEmpty(att.Value)) ? null as DateTime? : DateTime.Parse(att.Value);
        }

    }
}
