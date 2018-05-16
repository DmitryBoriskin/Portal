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
                backModel.Item.Id = id;
                string path = $"{Settings.UserFiles}{SiteDir}{Settings.PhotoDir}{id}/";

                if (uploads != null)
                {
                    int counter = 0;
                    PhotoModel[] photoList = new PhotoModel[uploads.Count()];
                    foreach (var photo in uploads)
                    {
                        if (photo != null && photo.ContentLength > 0)
                        {
                            if (!allowedExtention.Contains(Path.GetExtension(photo.FileName.ToLower())))
                            {

                            }
                            if (!Directory.Exists(Server.MapPath(path)))
                            {
                                Directory.CreateDirectory(Server.MapPath(path));
                            }

                            double filesCount = Directory.EnumerateFiles(Server.MapPath(path)).Count();
                            double newFilenameInt = Math.Ceiling(filesCount / 2) + 1;
                            string newFilename = newFilenameInt.ToString() + ".jpg";

                            while (System.IO.File.Exists(Server.MapPath(Path.Combine(path, newFilename))))
                            {
                                newFilenameInt++;
                                newFilename = newFilenameInt.ToString() + ".jpg";
                            }

                            //сохраняем оригинал
                            photo.SaveAs(Server.MapPath(Path.Combine(path, newFilename)));

                            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                            EncoderParameters myEncoderParameters = new EncoderParameters(1);
                            myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L); //cжатие 90

                            Bitmap _File = (Bitmap)Bitmap.FromStream(photo.InputStream);

                            //оригинал
                            Bitmap _FileOrigin = Imaging.Resize(_File, 4000, "width");
                            _FileOrigin.Save(Server.MapPath($"{path}/{newFilename}"), myImageCodecInfo, myEncoderParameters);

                            //сохраняем full hd
                            Bitmap _FileHd = Imaging.Resize(_File, 2000, "width");
                            _FileHd.Save(Server.MapPath($"{path}/hd_{newFilename}"), myImageCodecInfo, myEncoderParameters);

                            //сохраняем превью
                            Bitmap _FilePrev = Imaging.Resize(_File, 120, 120, "center", "center");
                            _FilePrev.Save(Server.MapPath($"{path}/prev_{newFilename}"), myImageCodecInfo, myEncoderParameters);

                            photoList[counter] = new PhotoModel()
                            {
                                Id = Guid.NewGuid(),
                                Album = id,
                                Title = Path.GetFileName(photo.FileName),
                                Date = DateTime.Now,
                                Preview = $"{path}/prev_{newFilename}",
                                Url = $"{path}/{newFilename}",
                                Sort = counter + 1
                            };
                            counter++;
                        }
                    }
                    _cmsRepository.InsertPhotos(id, photoList);
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

            model.ErrorInfo = message;
            return View("item", model);
        }
    }
}