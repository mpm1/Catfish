﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Catfish.Core.Models.Contents.ViewModels.ListEntries
{
    public class EntityListEntry
    {
        public Guid Id { get; set; }
        public string ModelType { get; set; }
        public MultilingualText Name { get; protected set; }
        public MultilingualText Description { get; protected set; }
        public Guid? PrimaryCollectionId { get; set; }
        public MultilingualText PrimaryCollectionName { get; set; }

        public EntityListEntry(Entity entity)
        {
            Id = entity.Id;
            ModelType = entity.ModelType;
            Name = entity.Name;
            Description = entity.Description;
            PrimaryCollectionId = entity.PrimaryCollectionId;
            PrimaryCollectionName = entity.PrimaryCollection != null ? entity.PrimaryCollection.Name : null;
        }

    }
}
