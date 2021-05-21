using System;
using System.Collections.Generic;
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
