using WEB.Models;
using Communication.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WEB.Controllers
{

    // the home controller for all the views.
    public class HomeController : Controller
    {
        private static ConfigModel config = new ConfigModel();
        private static LogsModel logs = new LogsModel();
        private static PhotosModel photos = new PhotosModel(config);
        private static ImageWebModel imageWeb = new ImageWebModel();
        private static string m_handlerRequested = null;
        // the controller for the view of the config settings
        public ActionResult Config()
        {
            ViewBag.Message = "The App Configuration.";
            config.SendConfigRequest();
            
            return View(config);
        }
        // the controller for the view of the main image web page
        public ActionResult ImageWeb()
        {
            ViewBag.Message = "The main home page.";
            ViewBag.IsConnected = imageWeb.IsConnected;
            photos.SetPhotos();
            imageWeb.NumOfPics = photos.NumOfPics;
            ViewBag.NumOfPics = imageWeb.NumOfPics;
            return View(imageWeb);
        }
        // the controller for the view of the logs page
        public ActionResult Logs()
        {
            ViewBag.Message = "The list of service logs.";
            logs.SendLogRequest();
            
            return View(logs);
        }
        // the controller for the view of the photos display
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
            m_handlerRequested = handlerToRemove;
            return View();
        }
        // invoked when the ok button has been selected to delete the handler, calls the function to delete the handler and returns to the view
        public ActionResult DeleteOK()
        {
            config.RemoveHandler(m_handlerRequested);
            return RedirectToAction("Config");
        }
        // invoked when the cancel button has been selected to delete the handler, returns to the config page
        public ActionResult DeleteCancel()
        {
            return RedirectToAction("Config");
        }
        // the controller for the photo viewer page, sends the photo to view
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
        // Deletes the specific photo.
        // <param name="fullUrl">The full URL.</param>
        public ActionResult DeleteSpecificPhoto(string fullUrl)
        {
            photos.DeletePhoto(fullUrl);
            return RedirectToAction("Photos");
        }
    }
}