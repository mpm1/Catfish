﻿using Catfish.Core.Models;
using Catfish.Core.Models.Data;
using Catfish.Core.Models.Forms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Catfish.Core.Services
{
    public class ItemService: EntityService
    {
        public ItemService(CatfishDbContext db) : base(db) { }

        public string UploadRoot { get { return ConfigurationManager.AppSettings["UploadRoot"]; } }

        public string DataRoot { get { return Path.Combine(UploadRoot, "Data"); } }

        public string GetURL(string pathName)
        {
            var uri = new System.Uri(pathName);
            var converted = uri.AbsoluteUri;
            return converted;
        }

        protected string CreateGuidName(string baseName)
        {
            string filename = Guid.NewGuid().ToString().Replace("-", "_");
            var idx = baseName.LastIndexOf(".");
            if (idx > 0)
                filename = filename + "." + baseName.Substring(idx + 1);
            return filename;
        }

        protected List<DataFile> UploadFiles(HttpRequestBase request, string dstPath)
        {
            dstPath = Path.Combine(UploadRoot, dstPath);
            if (!Directory.Exists(dstPath))
            {
                Directory.CreateDirectory(dstPath);
                if (!Directory.Exists(dstPath))
                    throw new Exception("Unable to create the upload folder " + dstPath);
            }

            List<DataFile> newFiles = new List<DataFile>();
            for (int i = 0; i < request.Files.Count; i++)
            {
                DataFile file = new DataFile();

                file.FileName = request.Files[i].FileName;
                file.GuidName = CreateGuidName(file.FileName);
                file.Path = dstPath;
                file.ContentType = request.Files[i].ContentType;
                file.Thumbnail = GetThumbnail(file.ContentType);
                file.ThumbnailType = DataFile.eThumbnailTypes.Shared;

                request.Files[i].SaveAs(Path.Combine(UploadRoot, file.Path, file.GuidName));

                newFiles.Add(file);
            }

            return newFiles;
        }

        public List<DataFile> UploadTempFiles(HttpRequestBase request)
        {
            List<DataFile> files = UploadFiles(request, "temp-files");
            Db.XmlModels.AddRange(files);
            return files;
        }

        public List<DataFile> UploadFiles(int itemId, HttpRequestBase request)
        {
            Item parent = Db.Items.Where(i => i.Id == itemId).FirstOrDefault();
            if (parent == null)
                throw new Exception("Parent item not found");

            if (string.IsNullOrEmpty(parent.Guid))
                parent.Guid = Guid.NewGuid().ToString("N");

            string dstPath = Path.Combine("data", parent.Guid);
            List<DataFile> newFiles = UploadFiles(request, dstPath); 
            foreach(DataFile file in newFiles)
                parent.AddData(file);

            Db.Entry(parent).State = EntityState.Modified;
            return newFiles;
        }

        public DataFile GetFile(int id, string name, bool checkInItems = true)
        {
            XmlModel model = Db.XmlModels.Find(id);
            if (model is DataFile)
                return model as DataFile;
            else if (checkInItems && model is Item)
                return (model as Item).Files.Where(f => f.GuidName == name).FirstOrDefault();
            else
                return null;
        }

        public bool DeleteFile(int itemId, string guidName)
        {
            Item item = Db.Items.Where(i => i.Id == itemId).FirstOrDefault();
            if (item == null)
                throw new Exception("Item not found");

            //check if the file is referreed by any form in this item
            bool referenced = false;
            foreach(var form in item.FormSubmissions)
            {
                if (referenced = form.FormData.CheckFileReference(guidName))
                    break;
            }

            if (referenced)
                return false;

            item.RemoveFile(guidName);
            Db.Entry(item).State = EntityState.Modified;
            return true;
        }

        public void DeleteFile(DataFile file)
        {

        }

        public Item UpdateStoredItem(Item changedItem)
        {
            Item dbModel = new Item();

            if (changedItem.Id > 0)
            {
                dbModel = Db.XmlModels.Find(changedItem.Id) as Item;
                //dbModel.Deserialize();

                //updating the "value" text elements
                dbModel.UpdateValues(changedItem);

                //NOTE: Do not change files here. They are added and deleted through AJAX API calls, not when the item is saved.

            }
            else
            {
                dbModel = CreateEntity<Item>(changedItem.EntityTypeId.Value);
                dbModel.UpdateValues(changedItem);
            }

            //Processing any file attachments that have been submitted
            //========================================================
            List<string> keepFileGuids = changedItem.AttachmentField.FileGuids.Split(new char[] { Attachment.FileGuidSeparator }, StringSplitOptions.RemoveEmptyEntries).ToList();

            //Removing attachments that are in the dbModel but not in attachments to be kept
            foreach(DataFile file in dbModel.Files.ToList())
            {
                if (keepFileGuids.IndexOf(file.GuidName) < 0)
                    DeleteFile(file);
            }

            //Adding new files
            foreach(string fileGuid in keepFileGuids)
            {
                if(dbModel.Files.Where(f => f.GuidName == fileGuid).Any() == false)
                {
                    DataFile file = Db.XmlModels.Where(m => m.Guid == fileGuid)
                        .Select(m => m as DataFile)
                        .FirstOrDefault();

                    if (file != null)
                    {
                        dbModel.AddData(file);
                        //since the data object has now been inserted into the submission item, it is no longer needed 
                        //to stay as a stanalone object in the XmlModel table.
                        Db.XmlModels.Remove(file);

                        //moving the physical files from the temporary upload folder to a folder identified by the GUID of the
                        //item inside the uploaded data folder
                        string dstDir = Path.Combine(DataRoot, dbModel.Guid);
                        if (!Directory.Exists(dstDir))
                            Directory.CreateDirectory(dstDir);

                        string srcFile = Path.Combine(file.Path, file.GuidName);
                        string dstFile = Path.Combine(dstDir, file.GuidName);
                        File.Move(srcFile, dstFile);

                        //moving the thumbnail, if it's not a shared one
                        if(file.ThumbnailType == DataFile.eThumbnailTypes.NonShared)
                        {
                            string srcThumbnail = Path.Combine(file.Path, file.Thumbnail);
                            string dstThumbnail = Path.Combine(dstDir, file.Thumbnail);
                            File.Move(srcThumbnail, dstThumbnail);
                        }

                        //updating the file path
                        file.Path = dstDir;
                    }
                }
            }
            //End: processing file attachments

            if (changedItem.Id > 0) //update Item
                Db.Entry(dbModel).State = EntityState.Modified;
            else
                Db.XmlModels.Add(dbModel);

            return dbModel;
        }


        ////public Item UpdateStoredItem(Item changedItem)
        ////{
        ////    Item dbModel = new Item();

        ////    if (changedItem.Id > 0)
        ////    {
        ////        dbModel = Db.XmlModels.Find(changedItem.Id) as Item;
        ////        dbModel.Deserialize();

        ////        //updating the "value" text elements
        ////        dbModel.UpdateValues(changedItem);

        ////        //Deleting files in the dbModel item if those files are not in the changedItem
        ////        if (dbModel.Files.Any()) //add checking if item contain any files
        ////        {
        ////            foreach (DataFile df in dbModel.Files)
        ////            {
        ////                if (changedItem.Files.Where(f => f.GuidName == df.GuidName).Any() == false)
        ////                {
        ////                    df.Data.Remove();
        ////                    DeleteFile(df);
        ////                }
        ////            }
        ////        }

        ////        //Inserting new files that are in the source item that are still not in this item
        ////        if (changedItem.Files.Any())
        ////        {
        ////            foreach (DataFile df in changedItem.Files)
        ////            {
        ////                if (dbModel.Files.Where(f => f.GuidName == df.GuidName).Any() == false)
        ////                {
        ////                    //since the posted model doesn't have the full property list passed back from the POST call, 
        ////                    //we should reload it form the database and use it.
        ////                    XmlModel tmp_file_model = Db.XmlModels.Where(m => m.Guid == df.GuidName).FirstOrDefault();
        ////                    tmp_file_model.Deserialize();

        ////                    dbModel.InsertChildElement("./files", tmp_file_model.Data);

        ////                    //since we inserted the XML data of df into the XML model of the item,
        ////                    //we no longer need to keep it in the database table. Howeber, we DO NEED to keep the files
        ////                    //because these files are now referred by the XML File model which was inserted into the XML Item model.
        ////                    //Deleting the File table entry corresponding to df
        ////                    Db.XmlModels.Remove(Db.XmlModels.Find(tmp_file_model.Id));
        ////                }
        ////            }
        ////        }

        ////    }
        ////    else{
        ////        dbModel = changedItem;
        ////    }


        ////    if (changedItem.Id > 0) //update Item
        ////        Db.Entry(dbModel).State = EntityState.Modified;
        ////    else
        ////        Db.XmlModels.Add(dbModel);

        ////    return dbModel;
        ////}

        public string GetThumbnail(string contentType)
        {
            if (contentType == "application/pdf")
                return "pdf.png";

            if (contentType == "application/msword")
                return "doc.png";

            if (contentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                return "docx.png";

            if (contentType == "application/vnd.ms-excel")
                return "xls.png";

            if (contentType == "application/x-zip-compressed")
                return "zip.png";

            if (contentType == "image/jpeg")
                return "jpg.png";

            if (contentType == "image/png")
                return "png.png";

            if (contentType == "image/tiff")
                return "tiff.png";

            if (contentType == "text/html")
                return "html.png";

            if (contentType == "text/xml")
                return "xml.png";

            return "other.png";
        }
    }
}
