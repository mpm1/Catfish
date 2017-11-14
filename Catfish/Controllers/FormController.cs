﻿using Catfish.Core.Models;
using Catfish.Core.Services;
using Catfish.Models.Regions;
using Piranha.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Catfish.Controllers
{
    public class FormController : SinglePageController
    {
        // GET: Forms
        public ActionResult Index()
        {
            var model = GetModel();
            ViewBag.PageModel = model;

            Form form = model.Regions.Form as Form;
            int entityTypeId = form.EntityTypeId;

            CatfishDbContext db = new CatfishDbContext();
            ItemService srv = new ItemService(db);
            Item item = srv.CreateEntity<Item>(entityTypeId);

            return View(model.GetView(), item);
        }

        [HttpPost]
        public ActionResult Edit(Item item)
        {
            CatfishDbContext db = new CatfishDbContext();
            var model = GetModel();

            if (ModelState.IsValid)
            {
                ItemService srv = new ItemService(db);
                db.SaveChanges();

                string confirmLink = "confirmation";
                return Redirect(confirmLink);
            }

            ViewBag.PageModel = model;
            return View(model.GetView(), item);
        }

        public ActionResult Confirmation()
        {
            var model = GetModel();
            return View(model);
        }
    }
}