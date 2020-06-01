﻿using Catfish.Core.Models;
using Catfish.Core.Models.Contents;
using SolrNet.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Catfish.Core.Models.Solr
{
    public class SolrItemModel
    {
        public SolrItemModel() { }

        public SolrItemModel(Item model)
        {
            Id = model.Id;
            Name = model.Name;
            Description = model.Description;
            Content = model.Content;
            Created = model.Created;
            Updated = model.Updated;
            MetadataSets = model.MetadataSets;
            PrimaryCollectionId = model.PrimaryCollectionId;
        }
        [SolrUniqueKey("id")]
        public Guid Id { get; set; }

        [SolrField("content")]
        [Column(TypeName = "xml")]
        public string Content { get; set; }

        [SolrField("name")]
        public MultilingualText Name { get; set; }

        [SolrField("description")]
        public MultilingualText Description { get; set; }

        [SolrField("metadataSets")]
        public XmlModelList<MetadataSet> MetadataSets { get; set; }

        [SolrField("created")]
        public DateTime? Created { get; set; }

        [SolrField("updated")]
        public DateTime? Updated { get; set; }
                
        [SolrField("primaryCollectionId")]
        public Guid? PrimaryCollectionId { get; set; }
    }
}
