﻿using System;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.AttributeBuilder;
using Piranha.Models;


namespace Catfish.Models.Blocks
{
    [BlockType(Name = "Embed Block", Category = "Content", Component = "embed-block",  Icon = "fas fa-code")]
    public class EmbedBlock : Block
    {
       // public TextField Source { get; set; }
        public TextField Embed { get; set; }


       //public override string GetTitle()
       // {
       //     if (Source.Value != null)
       //         return Source.Value;

       //     return "";
       // }

        public string GetEmbedCode()
        {
            if(Embed.Value != null)
            {

                return Embed.Value;
            }
            return "";
        }
       
    }
}
