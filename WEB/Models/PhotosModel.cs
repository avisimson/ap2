using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace WEB.Models
{

    public class PhotosModel
    {

        private string outputDir;
        private ConfigModel config;
        

        public PhotosModel(ConfigModel config)
        {
            ImageList = new List<Photo>();
            this.config = config;
            if (!config.Requested)
            {
                config.SendConfigRequest();
            }
            outputDir = config.OutputDirectory;

        }


        public List<Photo> ImageList
        {
            get; set;
        }

        //return the num of pics in the imageList
        public int NumOfPics
        {
            get
            {
                return this.ImageList.Count;
            }
        }

        
        //Sets the photos from the output directory.
        public void SetPhotos()
        {
            try
            {
                string thumbnailDir = outputDir + "\\Thumbnails";
                //check if exist folder that her name is "Thumbnails"
                if (!Directory.Exists(thumbnailDir))
                {
                    return;
                }
                DirectoryInfo di = new DirectoryInfo(thumbnailDir);

                string[] validExtensions = { ".jpg", ".png", ".gif", ".bmp" };
                foreach (DirectoryInfo yearDirInfo in di.GetDirectories())
                {
                    if (!Path.GetDirectoryName(yearDirInfo.FullName).EndsWith("Thumbnails"))
                    {
                        continue;
                    }
                    foreach (DirectoryInfo monthDirInfo in yearDirInfo.GetDirectories())
                    {
                        foreach (FileInfo fileInfo in monthDirInfo.GetFiles())
                        {
                            if (validExtensions.Contains(fileInfo.Extension.ToLower()))
                            {
                                Photo im = ImageList.Find(x => (x.ImageFullThumbnailUrl == fileInfo.FullName));
                                if (im == null)
                                {
                                    ImageList.Add(new Photo(fileInfo.FullName));
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        public void DeletePhoto(string fullUrl)
        {
            try
            {
                foreach (Photo photo in ImageList)
                {
                    if (photo.ImageFullUrl.Equals(fullUrl))
                    {
                        File.Delete(photo.ImageFullUrl);
                        File.Delete(photo.ImageFullThumbnailUrl);
                        this.ImageList.Remove(photo);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}