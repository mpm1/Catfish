﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Catfish.Core.Models.Contents.Fields
{
    public class Option : XmlModel
    {
        public const string FieldTagName = "option";

        public MultilingualName OptionText { get; set; }
        public Guid Id
        {
            get => Guid.Parse(Data.Attribute("id").Value);
            set => Data.SetAttributeValue("id", value);
        }
        public bool Selected
        {
            get => GetAttribute("selected", false);
            set => SetAttribute("selected", value);
        }

        public Option() : base(FieldTagName) { }
        public Option(XElement data) : base(data) { }

        public override void Initialize(eGuidOption guidOption)
        {
            //Ensuring that each field has a unique ID
            base.Initialize(guidOption == eGuidOption.Ignore ? eGuidOption.Ensure : guidOption);

            OptionText = new MultilingualName(GetElement(MultilingualName.TagName, true));
        }

        public void SetOptionText(string text, string lang)
        {
            OptionText.SetContent(text, lang);
        }
    }
}
