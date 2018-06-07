using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Text.RegularExpressions;

namespace WEB.Models
{
    public class PhotosModel
    {
        //fields to present a photo in web app.   

        [Required]
        [DataType(DataType.ImageUrl)]
        [Display(Name = "CurrentImage")]
        public Image currentImage { get; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ImagePath")]
        public string imgPath { get; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Year")]
        public string year { get; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Month")]
        public string month { get; }
        /*
         * constructor
         * param name = path, the path in cp to photo.
         */
        public PhotosModel(string path)
        {
            //set us to directory of current photo
            string root = Path.GetPathRoot(path);
            Directory.SetCurrentDirectory(root);
            //initialize class vars.
            imgPath = path;
            currentImage = Image.FromFile(path);
            month = Path.GetDirectoryName(path);
            month = new DirectoryInfo(month).Name;
            year = Path.GetDirectoryName(Path.GetDirectoryName(path));
            year = new DirectoryInfo(year).Name;
        }
    }
}