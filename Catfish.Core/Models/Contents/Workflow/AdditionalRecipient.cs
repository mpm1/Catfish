using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Catfish.Core.Models.Contents.Workflow
{
    public class AdditionalRecipient : EmailRecipient
    {
        public static readonly string TagName = "additional-recipient";

        public AdditionalRecipient(XElement data)
            : base(data)
        {

        }

        public AdditionalRecipient()
            : base(new XElement(TagName))
        {

        }
    }
}
