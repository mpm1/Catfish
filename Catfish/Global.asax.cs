﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using Piranha.WebPages;

namespace Catfish
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //Adding menu items
            var menubar = Manager.Menu.Where(m => m.InternalId == "Content").FirstOrDefault();
            var idx = 0;
            menubar.Items.Insert(idx++, new Manager.MenuItem()
              {
                  Name = "Metadata",
                  Action = "index",
                  Controller = "metadata",
                  Permission = "ADMIN_CONTENT"
                  //,SelectedActions = "productlist,productedit"
              });
        }
    }
}