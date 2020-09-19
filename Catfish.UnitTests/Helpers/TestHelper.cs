﻿using Catfish.Core.Models;
using Catfish.Core.Models.Solr;
using Catfish.Core.Services;
using Catfish.Core.Services.Solr;
using Catfish.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.AspNetCore.Identity.SQLServer;
using Piranha.Data.EF.SQLServer;
using Piranha.Services;
using SolrNet;
using System;

namespace Catfish.Tests.Helpers
{
    public class TestHelper
    {
        public IServiceProvider Seviceprovider { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputPath"></param>
        /// <returns></returns>
        public TestHelper()
        {
            //Creating a service collection
            var services = new ServiceCollection();

            //Registering configuration object
            IConfiguration configuration = new ConfigurationBuilder()
                .Add(new JsonConfigurationSource() { Path = "appsettings.test.json" })
                .Build();
            services.AddSingleton(typeof(IConfiguration), configuration);

            services.AddPiranhaEF<SQLServerDb>(options =>
                options.UseSqlServer(configuration.GetConnectionString("catfish")));
            services.AddPiranhaIdentityWithSeed<IdentitySQLServerDb>(options =>
                options.UseSqlServer(configuration.GetConnectionString("catfish")));

            //Registering application DB Context
            string dbConnectionString = configuration.GetConnectionString("catfish");
            services.AddDbContext<AppDbContext>(options => options
                .UseSqlServer(dbConnectionString)
                );
            services.AddDbContext<PiranhaDbContext>(options => options
                .UseSqlServer(dbConnectionString)
                );

            //Additiona Piranha Services
            services.AddScoped<IApi, Piranha.Api>();
            services.AddScoped<ISiteService, Piranha.Services.SiteService>();
            services.AddScoped<IPageService, Piranha.Services.PageService>();
            services.AddScoped<IParamService, ParamService>();
            services.AddScoped<IMediaService, Piranha.Services.MediaService>();
            services.AddScoped<IStorage, Piranha.Local.FileStorage>();
            services.AddScoped<ISiteService, SiteService>();


            //Registering other services
            services.AddScoped<SeedingService>();
            services.AddScoped<DbEntityService>();
            services.AddScoped<IEntityService, EntityService>();
            services.AddTransient<IWorkflowService, WorkflowService>();
            services.AddTransient<IAuthorizationService, AuthorizationService>();


            ////services.AddScoped<SolrService>();
            // Solr services
            string solrString = configuration.GetSection("SolarConfiguration:solrItemURL").Value;
            services.AddSolrNet<SolrItemModel>(solrString);
            services.AddScoped<ISolrIndexService<SolrItemModel>, SolrIndexService<SolrItemModel, ISolrOperations<SolrItemModel>>>();

            //Creating a service provider and assigning it to the member variable so that it can be used by 
            //test methods.
            Seviceprovider = services.BuildServiceProvider();
        }

        public AppDbContext Db
        {
            get => Seviceprovider.GetService<AppDbContext>();
        }

        public IWorkflowService WorkflowService
        {
            get => Seviceprovider.GetService<IWorkflowService>();
        }

        public IAuthorizationService AuthorizationService
        {
            get => Seviceprovider.GetService<IAuthorizationService>();
        }
    }
}
