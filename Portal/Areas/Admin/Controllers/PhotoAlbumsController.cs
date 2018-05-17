using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    public class PhotoAlbumsController : CoreController
    {
        PhotoViewModel model;
        FilterModel filter;
        string[] allowedExtention = Settings.PicTypes.Split(',')
                                                     .Where(w => !String.IsNullOrWhiteSpace(w))
                                                     .ToArray();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new PhotoViewModel()
            {
                PageName = PageName,
                Account = AccountInfo,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                UserResolution = UserResolutionInfo,
                ActionName = ActionName
            };
            if (AccountInfo != null)
            {
                model.Menu = MenuCmsCore;
                model.MenuModul = MenuModulCore;
            }
        }

        // GET: Admin/PhotoAlbums
        public ActionResult Index()
        {
            filter = GetFilter();
            model.List = _cmsRepository.GetPhotoAlbums(filter);
            return View(model);
        }

        // GET: Admin/PhotoAlbums/<id>
        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetPhotoAlbum(id);
            ViewBag.PicTypes = String.Join(",", allowedExtention.Select(s => $".{s}"));
            if (model.Item != null)
            {
                ViewBag.Date = DateTime.Now;
                ViewBag.Preview = model.Item.Preview;
            }
            return View("Item", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "insert-btn")]
        public ActionResult Insert()
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "page", String.Empty);

            return Redirect($"{StartUrl}item/{Guid.NewGuid()}/{query}");
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid id, PhotoViewModel backModel, HttpPostedFileBase upload, IEnumerable<HttpPostedFileBase> uploads)
        {
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация"
            };
            if (ModelState.IsValid)
            {
                bool status = false;
                backModel.Item.Id = id;
                string path = $"{Settings.UserFiles}{SiteDir}{Settings.PhotoDir}{id}/";

                if (upload != null && upload.ContentLength > 0)
                {
                    if (!allowedExtention.Contains(upload.FileName.Substring(upload.FileName.LastIndexOf('.') + 1)))
                    {
                        model.ErrorInfo = new ErrorMessage()
                        {
                            Title = "Ошибка",
                            Info = "Вы не можете загружать файлы данного формата",
                            Buttons = new ErrorMessageBtnModel[]
                            {
                                new ErrorMessageBtnModel { Url = "#", Text = "ок", Action = "false", Style="primary" }
                            }
                        };
                        return View("Item", model);
                    }
                    backModel.Item.Preview = SavePreviewAlbum(upload, path);
                }

                if (_cmsRepository.CheckPhotoAlbumExists(id))
                {
                    status = _cmsRepository.UpdatePhotoAlbum(backModel.Item);
                    message.Info = "Запись обновлена";
                }
                else
                {
                    status = _cmsRepository.InsertPhotoAlbum(backModel.Item);
                    message.Info = "Запись сохранена";
                }

                if (uploads != null && uploads.Any(a => a != null))
                {
                    var helper = new UploadImageHelper
                    {
                        Uploads = uploads,
                        Path = path,
                        AlbumId = id,
                        IsNeedPreview = String.IsNullOrWhiteSpace(backModel.Item.Preview)
                    };
                    string prev = SaveUploadImages(helper);
                    if (!String.IsNullOrWhiteSpace(prev))
                    {
                        _cmsRepository.UpdatePhotoAlbumPreview(id, prev);
                    }
                }

                message.Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = $"{StartUrl}{Request.Url.Query}", Text = "вернуться в список" },
                    new ErrorMessageBtnModel { Url = $"{StartUrl}item/{id}", Text = "ок", Action = "false" }
                };
            }
            else
            {
                message.Info = "Ошибка в заполнении формы. Поля, в которых допущены ошибки помечены цветом.";
                message.Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = "#", Text = "ок", Action = "false" }
                };
            }

            model.Item = _cmsRepository.GetPhotoAlbum(id);
            model.ErrorInfo = message;
            return View("item", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel()
        {
            return Redirect($"{StartUrl}{Request.Url.Query}");
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            _cmsRepository.DeletePhotoAlbum(id);
            string path = $"{Settings.UserFiles}{SiteDir}{Settings.PhotoDir}{id}/";

            DirectoryInfo dir = new DirectoryInfo(Server.MapPath(path));
            if (dir.Exists)
            {
                dir.Delete(true);
            }

            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация",
                Info = "Запись удалена",
                Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = $"{StartUrl}{Request.Url.Query}", Text = "ок", Action = "false" }
                }
            };

            model.ErrorInfo = message;
            return RedirectToAction("index");
        }

        [HttpPost]
        public string DeletePhoto(Guid id)
        {
            PhotoCoreModel result = _cmsRepository.DeletePhoto(id);
            if (result != null)
            {
                string path = Server.MapPath($"{Settings.UserFiles}{SiteDir}{Settings.PhotoDir}{result.Album}/");
                DirectoryInfo dir = new DirectoryInfo(path);
                if (dir.Exists)
                {
                    FileInfo[] files = new FileInfo[] 
                    {
                        new FileInfo($"{path}{result.Title}"),
                        new FileInfo($"{path}prev_{result.Title}"),
                        new FileInfo($"{path}hd_{result.Title}")
                    };
                    
                    foreach (var file in files)
                    {
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }
                    return "true";
                }
            }
            return "Не удалось удалить изображение";
        }

        /// <summary>
        /// Сохраняет превьюшку альбома
        /// </summary>
        /// <param name="upload"></param>
        /// <returns></returns>
        private string SavePreviewAlbum(HttpPostedFileBase upload, string path)
        {
            string[] defaultPreviewSizes = new string[] { "540", "360" };
            string fileExtension = upload.FileName.Substring(upload.FileName.LastIndexOf(".")).ToLower();

            string[] sizes = (!string.IsNullOrEmpty(Settings.MaterialPreviewImgSize)) ? Settings.MaterialPreviewImgSize.Split(',') : defaultPreviewSizes;
            int.TryParse(sizes[0], out int width);
            int.TryParse(sizes[1], out int height);
            return Files.SaveImageResizeRename(upload, path, "logo", width, height);
        }

        /// <summary>
        /// Сохраняет изображения для альбома
        /// </summary>
        /// <param name="uploads"></param>
        private string SaveUploadImages(UploadImageHelper helper)
        {
            int counter = 0;
            PhotoModel[] photoList = new PhotoModel[helper.Uploads.Count()];
            string albumPreview = null;
            foreach (var photo in helper.Uploads)
            {
                if (photo != null && photo.ContentLength > 0)
                {
                    if (allowedExtention.Contains(photo.FileName.Substring(photo.FileName.LastIndexOf('.') + 1)))
                    {
                        if (!Directory.Exists(Server.MapPath(helper.Path)))
                        {
                            Directory.CreateDirectory(Server.MapPath(helper.Path));
                        }
                        string newFilename = $"{(counter + 1).ToString()}.jpg";
                        
                        //сохраняем оригинал
                        photo.SaveAs(Server.MapPath(Path.Combine(helper.Path, newFilename)));

                        ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                        EncoderParameters myEncoderParameters = new EncoderParameters(1);
                        myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

                        Bitmap _File = (Bitmap)Bitmap.FromStream(photo.InputStream);

                        //оригинал
                        Bitmap _FileOrigin = Imaging.Resize(_File, 4000, "width");
                        _FileOrigin.Save(Server.MapPath($"{helper.Path}{newFilename}"), myImageCodecInfo, myEncoderParameters);

                        //сохраняем full hd
                        Bitmap _FileHd = Imaging.Resize(_File, 2000, "width");
                        _FileHd.Save(Server.MapPath($"{helper.Path}hd_{newFilename}"), myImageCodecInfo, myEncoderParameters);

                        //сохраняем превью
                        Bitmap _FilePrev = Imaging.Resize(_File, 120, 120, "center", "center");
                        _FilePrev.Save(Server.MapPath($"{helper.Path}prev_{newFilename}"), myImageCodecInfo, myEncoderParameters);

                        if (helper.IsNeedPreview && counter == 0)
                        {
                            albumPreview = SavePreviewAlbum(photo, helper.Path);
                        }

                        photoList[counter] = new PhotoModel()
                        {
                            Id = Guid.NewGuid(),
                            Album = helper.AlbumId,
                            Title = newFilename,
                            Date = DateTime.Now,
                            Preview = $"{helper.Path}prev_{newFilename}",
                            Url = $"{helper.Path}{newFilename}",
                            Sort = counter + 1
                        };
                        counter++;
                    }
                }
            }
            if (photoList != null && photoList.Any(a => a != null))
            {
                _cmsRepository.InsertPhotos(helper.AlbumId, photoList);
            }
            return albumPreview;
        }
    }
}