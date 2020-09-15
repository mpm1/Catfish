﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catfish.Core.Models;
using Catfish.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Catfish.Pages
{
    public class StartPageModel : CatfishPageModelModel
    {
        protected readonly IEntityTemplateService _entityTemplateService;
        public IList<ItemTemplate> ItemTemplates { get; set; }
        //public StartPageModel(IAuthorizationService auth, ISubmissionService serv, IEntityTemplateService temp)
        //    : base(auth, serv)
        public StartPageModel(IEntityTemplateService temp, IAuthorizationService auth)
            : base(null, null)
        {
            _entityTemplateService = temp;
        }

        public void OnGet()
        {
            ItemTemplates = _entityTemplateService.GetItemTemplates();
        }
    }
}