﻿using Catfish.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Catfish.Models.ViewModels
{
    public class CFAggregationIndexViewModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("mappedGuid")]
        public string MappedGuid { get; set; }

        [JsonProperty("entityType")]
        public string EntityType { get; set; }

        public CFAggregationIndexViewModel(CFAggregation aggregation)
        {
            Name = aggregation.Name;
            Id = aggregation.Id;
            MappedGuid = aggregation.MappedGuid;
            EntityType = aggregation.EntityType.Name;
        }
    }
}