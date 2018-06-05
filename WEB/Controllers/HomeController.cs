﻿using WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WEB.Controllers
{
    public class HomeController : Controller
    {
        private static ConfigModel config = new ConfigModel();
        private static LogsModel logs = new LogsModel();
        private static PhotosModel photos = new PhotosModel();
        private static ImageWebModel imageWeb = new ImageWebModel();

        public ActionResult Config()
        {
            ViewBag.Message = "The App Configuration.";
            config.SendConfigRequest();
            return View(config);
        }

        public ActionResult ImageWeb()
        {
            ViewBag.Message = "The main home page.";
            ViewBag.IsConnected = imageWeb.IsConnected;
            ViewBag.NumOfPics = imageWeb.NumOfPics;
            return View(imageWeb);
        }

        public ActionResult Logs()
        {
            ViewBag.Message = "The list of service logs.";
            logs.SendLogRequest();
            return View(logs);
        }

        public ActionResult Photos()
        {
            ViewBag.Message = "The photos saved.";
            return View(photos);
        }

        public ActionResult Confirm()
        {
            ViewBag.Message = "The photos saved.";

            return View(config);
        }

        public ActionResult Delete(string handlerToRemove)
        {
            config.RemoveHandler(handlerToRemove);
            return RedirectToAction("Config");
        }
    }
}