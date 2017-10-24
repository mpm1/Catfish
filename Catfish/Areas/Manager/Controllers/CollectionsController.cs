﻿using Catfish.Areas.Manager.Models.ViewModels;
using Catfish.Core.Models;
using Catfish.Core.Models.Metadata;
using Catfish.Core.Services;
using Catfish.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace Catfish.Areas.Manager.Controllers
{
    public class CollectionsController : CatfishController
    {
        private CatfishDbContext db = new CatfishDbContext();

        // GET: Manager/Collections
        public ActionResult Index()
        {
            var entities = db.XmlModels.Where(m => m is Collection).Include(e => (e as Entity).EntityType).Select(e => e as Entity);
            return View(entities);
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            Collection model = null;
            if (id.HasValue && id.Value > 0)
            {
                model = Db.Collections.Where(et => et.Id == id).FirstOrDefault();
                if (model != null)
                {
                    Db.Entry(model).State = EntityState.Deleted;
                    Db.SaveChanges();
                }
            }
            return RedirectToAction("index");
        }


        // GET: Manager/Collections/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Manager/Collections/Edit/5
        public ActionResult Edit(int? id, int? entityTypeId)
        {
            Collection model;
            CollectionService srv = new CollectionService(db);

            if (id.HasValue && id.Value > 0)
            {
                model = db.XmlModels.Find(id) as Collection;
                model.Deserialize();
            }
            else
            {
                if (entityTypeId.HasValue)
                {
                    model = srv.CreateEntity<Collection>(entityTypeId.Value);
                }
                else
                {
                    List<EntityType> entityTypes = srv.GetEntityTypes(EntityType.eTarget.Collections).ToList();
                    ViewBag.SelectEntityViewModel = new SelectEntityTypeViewModel()
                    {
                        EntityTypes = entityTypes
                    };

                    model = new Collection();
                }
            }

            return View(model);

            /***
                        Collection model;
                        CollectionService srv = new CollectionService(db);

                        if (id.HasValue)
                        {
                            model = db.XmlModels.Find(id) as Collection;
                            if (model == null)
                                return HttpNotFound();

                            List<EntityAssociationViewModel> associationList = new List<EntityAssociationViewModel>();
                            for (int n = 0; n < 2; ++n)
                            {
                                EntityAssociationViewModel childItems = new EntityAssociationViewModel();
                                int i = 1;
                                for (; i < 5; ++i)
                                    childItems.AssociatedEntities.Add(new EntityViewModel() { Id = i, Label = "Associated " + i });

                                for (; i < 10; ++i)
                                    childItems.AllEntities.Add(new EntityViewModel() { Id = i, Label = "All " + i });

                                associationList.Add(childItems);
                            }

                            ViewBag.associationList = associationList;

                            model.Deserialize();
                        }
                        else
                        {
                            string filename = "Collection.xml";
                            var path = WebConfigurationManager.AppSettings["TestDataFolder"];
                            path = Path.Combine(path, filename);
                            model = XmlModel.Load(path) as Collection;
                        }

                        //get entity type -- this won't be needed if from edit interface thereis a way to choose the EntityType
                        string eTypeName = model.Data.Attribute("entity-type").Value;
                        EntityType eType = db.EntityTypes.Where(e => e.Name == eTypeName).FirstOrDefault();
                        if (eType != null)
                        {
                            model.EntityTypeId = eType.Id;
                        }

                        ViewBag.JSONModel = Newtonsoft.Json.JsonConvert.SerializeObject(model.Data);
                        ////// Rendering these as json objects in view result in circular references 
                        //var metadataSets = db.MetadataSets.ToList();
                        ////var entityTypes = db.EntityTypes.ToList();
                        ////ViewBag.EntityTypes = new JavaScriptSerializer().Serialize(entityTypes);//Json(db.EntityTypes.ToList());
                        //ViewBag.MetadataSets = new JavaScriptSerializer().Serialize(metadataSets);//Json(db.MetadataSets.ToList());

                        ////for dropdown collection name
                        //var collections = db.Collections.AsEnumerable();
                        //var _collections = collections.Select(m => new SelectListItem
                        //{
                        //    Value = m.Id.ToString(),
                        //    Text = m.Name
                        //});
                        //ViewBag.Collections = _collections;

                        //populate WntityAssociationModel for knockout testing

                        return View("Edit", model);
            **/
        }

        // POST: Manager/Collections/Edit/5
        [HttpPost]
        public ActionResult Edit(Collection model)
        {
            if (ModelState.IsValid)
            {
                CollectionService srv = new CollectionService(db);

                Collection dbModel = srv.UpdateStoredCollection(model);
                db.SaveChanges();

                if (model.Id == 0)
                    return RedirectToAction("Edit", new { id = dbModel.Id });
                else
                    return View(dbModel);

            }
            return View(model);

            ////////if (ModelState.IsValid)
            ////////{
            ////////    ViewBag.Status = "Validation Passed";

            ////////    if (collection.Id > 0)
            ////////    { 
            ////////        Collection dbModel = UpdateStoredItem(collection);
            ////////        db.SaveChanges();

            ////////        return View("Edit", dbModel);
            ////////    }
            ////////    else
            ////////    {
            ////////        //get Parent CollectionId
            ////////        int parentColId = Convert.ToInt16(HttpContext.Request.Form["parentCollection"]);
            ////////        Collection parentCol = db.XmlModels.Find(parentColId) as Collection;

            ////////        collection.ParentMembers.Add(parentCol);
            ////////        //alternative 1
            ////////        // Collection dbModel = UpdateStoredItem(collection); // don't need anything else if query from db to get general structure of collection

            ////////        //TODO: Create a new service method on ItemService for the folowings, which returns a new Item
            ////////        // 1. Get the EntityType ID from the post call variable.
            ////////        // 2. Load the item type from the database
            ////////        // 3. Create a new item. Add the list of metadata sets in the item type into the newly created model
            ////////        // 4. Call srv.UpdateStoredItem(model); method to assign the values passed through the posted model into the newly created item
            ////////        //alternative 2
            ////////        Collection col = new Collection(); //for holding all xml labels that're non-existing in collection object that posted back, because it's contain only value
            ////////        col.Data.Add(new XElement("name", collection.Name)); //if it has its ownm <name> or using DC <title> field

            ////////        EntityType et = db.EntityTypes.Where(e => e.Id == collection.EntityTypeId).FirstOrDefault();
            ////////        XAttribute attribute = new XAttribute("entity-type", et.Name);
            ////////        col.Data.Add(attribute);
            ////////        XElement meta = col.Data.Element("metadata-sets");
            ////////        foreach (MetadataSet ms in et.MetadataSets)
            ////////        {
            ////////            MetadataSet mSet = db.XmlModels.Find(ms.Id) as MetadataSet;
            ////////            meta.Add(mSet.Data);
            ////////        }
            ////////        //update the values
            ////////        col.UpdateValues(collection);
            ////////        col.EntityTypeId = collection.EntityTypeId;

            ////////        //save the item
            ////////        db.Collections.Add(col);
            ////////        db.SaveChanges();
            ////////    }

               
            ////////}
            
            ////////return View();
        }

        public Collection UpdateStoredItem(Collection changedItem)
        { 
            Collection dbModel = db.XmlModels.Find(changedItem.Id) as Collection;
            dbModel.Deserialize();

            //updating the "value" text elements
            dbModel.UpdateValues(changedItem);
            db.Entry(dbModel).State = EntityState.Modified;
            
            return dbModel;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Item.Id or Collection.Id</param>
        /// <param name="offset"></param>
        /// <param name="maxPage"></param>
        /// <returns></returns>
        public JsonResult GetChildItems(int id, int offset=0, int maxPage=100)
        {
            List<EntityViewModel> lEntyties = new List<EntityViewModel>();
            var collection = db.XmlModels.Find(id) as Collection;

            int numItem = offset * maxPage;
            if(numItem <= 0)
            {
                numItem = collection.ChildItems.Count();
            }
            else
            {
                if (numItem > collection.ChildItems.Count())
                    numItem = collection.ChildItems.Count(); 
            }

            int i = 0;
            foreach(var item in collection.ChildItems)
            {
                EntityViewModel eVM = new EntityViewModel(item);
                lEntyties.Add(eVM);
                i++;
                if (i == numItem) //only retrieve as many item as indicate from offset and maxPage
                    break;
            }
            return Json(lEntyties);
        }

        public JsonResult GetChildCollections(int id, int offset = 0, int maxPage = 100)
        {
            List<EntityViewModel> lEntyties = new List<EntityViewModel>();
            var collection = db.XmlModels.Find(id) as Collection;

            int numItem = offset * maxPage;
            if (numItem <= 0)
            {
                numItem = collection.ChildItems.Count();
            }
            else
            {
                if (numItem > collection.ChildItems.Count())
                    numItem = collection.ChildItems.Count();
            }

            int i = 0;
            foreach (var col in collection.ChildCollections)
            {
                EntityViewModel eVM = new EntityViewModel(col);
                lEntyties.Add(eVM);
                i++;
                if (i == numItem)
                    break;
            }
            return Json(lEntyties);
        }
    }
}
