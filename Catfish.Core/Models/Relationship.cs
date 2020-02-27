﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Catfish.Core.Models
{
    public class Relationship
    {
        public string Predicate { get; set; }
        public int SubjectId { get; set; }
        public Aggregation2 Subject { get; set; }
        public int ObjctId { get; set; }
        public Aggregation2 Objct { get; set; }
    }
}
