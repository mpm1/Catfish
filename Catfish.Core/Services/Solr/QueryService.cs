﻿using Catfish.Core.Models;
using Catfish.Core.Models.Solr;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.DSL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Catfish.Core.Services.Solr
{
    public class QueryService : IQueryService
    {
        private readonly ISolrReadOnlyOperations<SolrItemModel> _solr;
        public QueryService(ISolrReadOnlyOperations<SolrItemModel> qrv)
        {
            _solr = qrv;
        }

        public SolrQueryResults<SolrItemModel> SimpleQueryByField1(string fieldname, string matchword)
        {
            SolrQueryResults<SolrItemModel> data = new SolrQueryResults<SolrItemModel>();
            var result = _solr.Query(new SolrQuery(fieldname + ":" + matchword)); // search for "matchword" in the "fieldname" field

            foreach (var item in result)
            {
                data.Add(item);
            }

            return data;
        }

        //public ISolrQuery BuildQuery(SearchParameters parameters)
        //{
        //    if (!string.IsNullOrEmpty(parameters.FreeSearch))
        //        return new SolrQuery(parameters.FreeSearch);
        //    return SolrQuery.All;
        //}
        public ISolrQuery BuildQuery(SearchParameters parameters)
        {
            var queryList = new List<ISolrQuery>();

            //Search for a given keyword in all configured Solr fields.
            queryList.Add(new SolrQueryByField("content", parameters.FreeSearch));

            
            return new SolrMultipleCriteriaQuery(queryList, "OR");
        }

        public SolrQueryResults<SolrItemModel> Search(SearchParameters parameters)
        {

            var solrQueryResults = _solr.Query(SolrQuery.All, new QueryOptions
            {
                FilterQueries = new Collection<ISolrQuery> { Query.Field("content").Is(parameters.FreeSearch) },
                Rows = parameters.PageSize,
                Start = parameters.PageIndex,
                //OrderBy = new Collection<SortOrder> { SortOrder.Parse("entityGuid asc") },
                //Facet = new FacetParameters
                //{
                //    Queries = new Collection<ISolrFacetQuery> { new SolrFacetFieldQuery("entityGuid") { MinCount = 1 } }
                //}
            });
            return solrQueryResults;
        }
        public SolrQueryResults<SolrItemModel> Results(SearchParameters parameters)
        {
            //QueryOptions query_options = new QueryOptions
            //{
            //    Rows = 10,
            //    StartOrCursor = new StartOrCursor.Start(0),
            //    FilterQueries = new ISolrQuery[] {
            //    new SolrQueryByField("content","provides"),
            //    }
            //};
            //// Construct the query
            //SolrQuery query = new SolrQuery("provides");
            //// Run a basic keyword search, filtering for questions only
            //var posts = _solr.Query(query, query_options);
            //SolrQueryResults<SolrItemModel> data = new SolrQueryResults<SolrItemModel>();
            //foreach (var item in posts)
            //{
            //    data.Add(item);
            //}
            SolrQueryResults<SolrItemModel> products2 = _solr.Query(new SolrQuery("content:provides"));

            return products2;
        }
        

    }
}
