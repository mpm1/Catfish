﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Catfish.Core.Models.Contents.Workflow
{
    public class PopUp : XmlModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public PopUp(XElement data)
            : base(data)
        {

        }
    }
}
