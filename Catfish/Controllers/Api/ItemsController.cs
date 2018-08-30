﻿using Catfish.Core.Models;
using Catfish.Core.Models.Data;
using Catfish.Core.Services;
using Catfish.Helpers;
using Catfish.Models.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Catfish.Services;
using Catfish.Core.Helpers;

namespace Catfish.Controllers.Api
{
    public class ItemsController : CatfishController
    {   
        public JsonResult GetPageItems(string q, int sortAttributeMappingId, bool sortAsc, int page, int itemPerPage, [Bind(Include = "mapIds[]")] int[] mapIds)
        {
            int total;
            var items = ItemService.GetPagedItems(q, sortAttributeMappingId, sortAsc, page, itemPerPage, out total);

            List<Tuple<int, List<string>>> result = new List<Tuple<int, List<string>>>(items.Count());

            List<string> mappings = new List<string>(mapIds.Length);
            foreach(int id in mapIds)
            {
                CFEntityTypeAttributeMapping am = EntityTypeService.GetEntityTypeAttributeMappingById(id);
                mappings.Add(am.Name);
            }

            foreach (var itm in items)
            {
                List<string> rowContent = new List<string>(mapIds.Length);
                foreach(string mapping in mappings)
                {
                    string content = itm.GetAttributeMappingValue(mapping);
                    rowContent.Add(content);
                }
                result.Add(new Tuple<int, List<string>>(itm.Id, rowContent));
            }

            return Json(new { total = total, result = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AutoCompleteField(string fieldId, string partialText, int rows = 10)
        {
            string jsonResult = SolrService.GetPartialMatichingText(fieldId, partialText, rows);

            return this.Content(jsonResult, "application/json");
        }

        public JsonResult GetGraphData(string q, string xMetadataSet, string xField, string yMetadataSet, string yField, string catMetadataSet, string catField, bool isCatOptionsIndex = false)
        {
            ItemQueryService itemQueryService = new ItemQueryService(Db);
            var result = itemQueryService.GetGraphData(q, xMetadataSet, xField, yMetadataSet, yField, catMetadataSet, catField, isCatOptionsIndex);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatsData(string q, ItemQueryService.eFunctionMode statMode, string selectedFieldMetadataSet, string selectedField)
        {
            decimal result = ItemQueryService.GetCalculatedField(q, statMode, selectedFieldMetadataSet, selectedField);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGraphData_old(string xMetadataSet, string xField, string yMetadataSet, string yField, string catMetadataSet, string catField,int xmin = 0, int xmax = 0)
        {
            ItemQueryService itemQueryService = new ItemQueryService(Db); 
            var result = itemQueryService.GetGraphData_old(xMetadataSet, xField, yMetadataSet, yField, catMetadataSet, catField, xmin, xmax);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: Items
        public JsonResult Index(string fields, int offset = 0, int limit = 25, bool randomize = false, int entityTypeId = 0, int collectionId = 0)
        {
            try
            {
                IEnumerable<CFItem> items;
                if (collectionId > 0)
                {
                    CFCollection collection = CollectionService.GetCollection(collectionId);
                    items = collection.ChildItems.Select(it => it as CFItem);
                }
                else
                {
                    items = ItemService.GetItems();
                }

                if (entityTypeId > 0)
                    items = items.Where(it => it.EntityTypeId == entityTypeId);

                items = randomize ? items.OrderBy(item => Guid.NewGuid()) : items.OrderBy(item => item.Id);

                if (offset > 0)
                    items = items.Skip(offset);

                if (limit > 0)
                    items = items.Take(limit);
                else
                    items = items.Take(25);

                List<BulletinBoardItem> ret = items.Select(it => new BulletinBoardItem(it, Request.RequestContext, fields)).ToList();
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                Response.StatusDescription = "An error occurred.";
                return Json(new List<BulletinBoardItem>(), JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult Thumbnail(int id, string name)
        {
            CFDataFile file = DataService.GetFile(id, name);
            if (file == null)
                return HttpNotFound("File not found");

            string path_name = file.ThumbnailType == CFDataFile.eThumbnailTypes.Shared
                ? Path.Combine(FileHelper.GetThumbnailRoot(Request), file.Thumbnail)
                : Path.Combine(file.Path, file.Thumbnail);

            return new FilePathResult(path_name, file.ContentType);
        }

        public ActionResult File(int id, string guid)
        {
            CFDataFile file = DataService.GetFile(id, guid);
            if (file == null)
                return HttpNotFound("File not found");

            string path_name = Path.Combine(file.Path, file.LocalFileName);
            return new FilePathResult(path_name, file.ContentType);
        }

        //August 1 2018 -- get image with different size
        /// <summary>
        /// If no size provided, it will return regular size image
        /// </summary>
        /// <param name="id">EntityId</param>
        /// <param name="guid">File Guid</param>
        /// <param name="size">image size (i.e: Thumbnail, small, medium, large)</param>
        /// <returns></returns>
        public ActionResult Image(int id, string guid, string size = null)
        {
            ConfigHelper.eImageSize? eSize = null;

            if (!string.IsNullOrEmpty(size))
            {
                eSize = (ConfigHelper.eImageSize)Enum.Parse(typeof(ConfigHelper.eImageSize), size);
            }

            CFDataFile file = null;
            if (!string.IsNullOrEmpty(guid))
            {
                file = DataService.GetFile(id, guid);    
            }
            if (file == null)
                return HttpNotFound("File not found");

            string path_name = string.Empty;
            string[] fnames = file.LocalFileName.Split('.'); 
            if (eSize == null)
            {
                path_name = Path.Combine(file.Path, file.LocalFileName);
            }
            else if (eSize.Equals(ConfigHelper.eImageSize.Thumbnail))
            {
                path_name = Path.Combine(file.Path, fnames[0] + "_t." + fnames[1]);
            }
            else if (eSize.Equals(ConfigHelper.eImageSize.Small))
            {
                path_name = Path.Combine(file.Path, fnames[0] + "_s." + fnames[1]);
            }
            else if (eSize.Equals(ConfigHelper.eImageSize.Medium))
            {
                path_name = Path.Combine(file.Path, fnames[0] + "_m." + fnames[1]);
            }
            else if (eSize.Equals(ConfigHelper.eImageSize.Large))
            {
                path_name = Path.Combine(file.Path, fnames[0] + "_l." + fnames[1]);
            }

            FilePathResult filePathResult = new FilePathResult(path_name, file.ContentType);
            return filePathResult; // Json(filePathResult, JsonRequestBehavior.AllowGet);
        }
    }
}