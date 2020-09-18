﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Catfish.Core.Models
{
    [Table("Catfish_Groups")]
    public class Group
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
    }
}
