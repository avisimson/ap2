using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace WEB.Models
{

    public class Photo
    {
        /*
         * constructor.
         * param name = imageUrl, the path to the specific photo.
         */
        public Photo(string imageUrl)
        {
            try
            {
                ImageFullThumbnailUrl = imageUrl;
                ImageFullUrl = imageUrl.Replace(@"Thumbnails\", string.Empty);
                Name = Path.GetFileNameWithoutExtension(ImageFullThumbnailUrl);
                Month = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(ImageFullThumbnailUrl));
                Year = Path.GetFileNameWithoutExtension(Path.GetDirectoryName((Path.GetDirectoryName(ImageFullThumbnailUrl))));
                string strDirName;
                int intLocation, intLength;

                intLength = imageUrl.Length;
                intLocation = imageUrl.IndexOf("OutputDir");

                strDirName = imageUrl.Substring(intLocation, intLength - intLocation);

                ImageRelativePathThumbnail = @"~\" + strDirName;
                ImageRelativePath = ImageRelativePathThumbnail.Replace(@"Thumbnails\", string.Empty);
            }
            catch (Exception e)
            {//case of failure in getting to directories of photo.
                Console.WriteLine(e.Message);
            }
        }
        //fields of details for photo.
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Year")]
        public string Year { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Month")]
        public string Month { get; set; }

        [Required]
        [DataType(DataType.ImageUrl)]
        [Display(Name = "ImageUrl")]
        public string ImageFullThumbnailUrl { get; set; }

        [Required]
        [DataType(DataType.ImageUrl)]
        [Display(Name = "ImageRelativePathThumbnail")]
        public string ImageRelativePathThumbnail { get; set; }

        [Required]
        [DataType(DataType.ImageUrl)]
        [Display(Name = "ImageRelativePath")]
        public string ImageRelativePath { get; set; }

        [Required]
        [DataType(DataType.ImageUrl)]
        [Display(Name = "ImageFullUrl")]
        public string ImageFullUrl { get; set; }
    }
}