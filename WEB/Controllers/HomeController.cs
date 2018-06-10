using WEB.Models;
using Communication.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Communication.Event;
using Communication.Enums;
using System.Threading;

namespace WEB.Controllers
{
    // the home controller for all the views. connect views with models.
    public class HomeController : Controller
    {
        private static string handlerPicked = null;
        //create all models for the web.
        private static ConfigModel config = new ConfigModel();
        private static LogsModel logs = new LogsModel();
        private static PhotosModel photos = new PhotosModel(config);
        private static ImageWebModel imageWeb = new ImageWebModel();
        // function activated when the config is pressed in views layout. get config request from service.
        // returns the config to the view.
        public ActionResult Config()
        {
            ViewBag.Message = "The App Configuration.";
            config.SendConfigRequest();

            return View(config);
        }
        // function activates image web page when activeted in layout. gets number of pic from dir.
        //returns the updated image web model to view.
        public ActionResult ImageWeb()
        {
            ViewBag.Message = "The main home page.";
            ViewBag.IsConnected = imageWeb.IsConnected;
            photos.SetPhotos();
            imageWeb.NumOfPics = photos.NumOfPics;
            ViewBag.NumOfPics = imageWeb.NumOfPics;
            return View(imageWeb);
        }
        // function activates logs page when activeted in layout. get logs from service.
        //returns the updated logs model to view.
        public ActionResult Logs()
        {
            ViewBag.Message = "The list of service logs.";
            logs.SendLogRequest();
            ViewBag.LogEntries = logs.LogEntries;
            return View(logs);
        }
        // function activates photos page when activeted in layout.
        //returns the updated photos model to view.
        public ActionResult Photos()
        {
            ViewBag.Message = "The photos saved.";
            photos.ImageList.Clear();
            photos.SetPhotos();
            return View(photos);
        }
        // the controller for when there has been a confirmation, returns to the config view
        public ActionResult Confirm()
        {
            ViewBag.Message = "The photos saved.";
            return View(config);
        }
        // sets the handler to be removed and removes to the view
        // <param name="handlerToRemove">The handler to remove.</param>
        public ActionResult ConfirmDeleteHandler(string handlerToRemove)
        {
            handlerPicked = handlerToRemove;
            return View();
        }
        // activated when the ok button has been selected to delete the handler
        //calls the function to delete the handler and returns to the view
        public ActionResult DeleteOK()
        {
            config.RemoveHandler(handlerPicked);
            return RedirectToAction("Config");
        }
        // activated when the cancel button has been selected to delete the handler, returns to the config page
        public ActionResult DeleteCancel()
        {
            return RedirectToAction("Config");
        }
        // the controller for the photo viewer page, sends the path of the photo to  the view.
        // <param name="fullUrl">The full URL.</param>
        public ActionResult PhotosViewer(string fullUrl)
        {
            Photo photo = new Photo(fullUrl);
            return View(photo);
        }
        // the controller for the photo deleting page, sends the photo to delete
        // <param name="fullUrl">The full URL.</param>
        public ActionResult PhotosDelete(string fullUrl)
        {
            Photo photo = new Photo(fullUrl);
            return View(photo);
        }
        // Deletes the specific photo, and return to photos page.
        // <param name="fullUrl">The full URL.</param>
        public ActionResult DeleteSpecificPhoto(string fullUrl)
        {
            photos.DeletePhoto(fullUrl);
            return RedirectToAction("Photos");
        }
    }
}