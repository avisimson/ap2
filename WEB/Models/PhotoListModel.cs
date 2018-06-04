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
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "PhotoPath")]
        public string PhotoPath { get; }

        [Required]
        [Display(Name = "PhotosList")]
        private List<PhotosModel> PhotosList = new List<PhotosModel>();

        public PhotoListModel(string path)
        {
            PhotoPath = path;
            RefreshList();
        }

        public void RefreshList()
        {
            //check if photopath exist
            if (Directory.Exists(PhotoPath))
            {
                //create a array of photos
                string[] photos = Directory.GetFiles(PhotoPath + "\\Thumbnails", "*.jpg", SearchOption.AllDirectories);
                foreach (string photo in photos)
                {
                    //add photo to list
                    PhotosList.Add(new PhotosModel(photo));
                }
            }
        }
        public List<PhotosModel> GetPhotos()
        {
            RefreshList();
            return PhotosList;
        }

        public int Length()
        {
            return PhotosList.Count;
        }

    }
}