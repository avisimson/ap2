using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WEB.Models
{
    public class PhotoListModel
    {
        //fields to show on web app.
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ImagePath")]
        public string imgPath { get; }

        [Required]
        [Display(Name = "photosList")]
        private List<PhotosModel> photosList = new List<PhotosModel>();
        //constructor.
        public PhotoListModel()
        {
            RefreshList();
        }
        /*
         * function check get to outputDir if it exists and insert all of its thumbnail
         * photos into a list.
         */
        public void RefreshList()
        {
            //check if output dir exist
            if (Directory.Exists(imgPath))
            {
                //initialize array of all thumbnail photos.
                string[] photos = Directory.GetFiles(imgPath + "\\Thumbnails", "*.jpg", SearchOption.AllDirectories);
                List<PhotosModel> tempList = new List<PhotosModel>();
                foreach (string photo in photos)
                {
                    tempList.Add(new PhotosModel(photo));
                }
                photosList = tempList;
            }
        }
        //returns list of photos after checking for changes.
        public List<PhotosModel> GetPhotos()
        {
            RefreshList();
            return photosList;
        }
        //returns the number of thumbnail photos that exist in list.
        public int Length()
        {
            return photosList.Count;
        }

    }
}