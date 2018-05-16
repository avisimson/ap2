using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing.Imaging;

namespace ImageService.Modal
{
    public class ImageServiceModal : IImageServiceModal
    {
        private static Regex r = new Regex(":"); //for creation time.
        private string m_OutputFolder { get; set; }            // The Output Folder
        private int m_thumbnailSize  {get; set;}            // The Size Of The Thumbnail Size
        /*
         * constructor
         * <param name = folder> -the source folder.
         * <param name = size> - the size of thumbnail.
         */
        public ImageServiceModal(string folder, int size)
        {
            this.m_OutputFolder = folder;
            this.m_thumbnailSize = size;
        }
        /// <summary>
        /// The Function Adds A file to the system
        /// </summary>
        /// <param name="path">The Path of the Image from the file</param>
        /// <returns>Indication if the Addition Was Successful</returns>
        public string AddFile(string path, out bool result)
        {
            if(File.Exists(path))
            {
                string[] paths = createDirectories(path); //create all directiories needed for file.
                string fileName = Path.GetFileName(path);
                //save the thumbnail.
                while (File.Exists(paths[1] + "//" + fileName))
                {
                    fileName = fileName + "1";
                }
                Image thumbnail;
                using (thumbnail = Image.FromFile(path))
                {
                    thumbnail = (Image)(new Bitmap(thumbnail, new Size(this.m_thumbnailSize, this.m_thumbnailSize)));
                    thumbnail.Save(paths[1] + "//" + fileName);
                }
                //save the file in the path.
                fileName = Path.GetFileName(path);
                while (File.Exists(paths[0] + "//" + fileName))
                {
                    fileName = fileName + "1";
                }
                File.Move(path, paths[0] + "//" + fileName);
                //file exist and moved to right path. return INFO.
                result = true;
                return "The file: " + Path.GetFileName(path) + " has been added to " + paths[0] +
                    " and to the thumbnails folder as" + fileName;
            } else {
                //the file doesn't exist. return FAIL
                result = false;
                return "file doesn't exist.";
            }   
        }
        /*
         * create all directories needed to put file in.
         * param name = path - path to directory of file.
         * returns- string array of 2 paths, for file and tumbnail file.
         */
        public string[] createDirectories(string path)
        {
            //create output directory if not already exists.
            DirectoryInfo dir = Directory.CreateDirectory(this.m_OutputFolder);
            //get creation time year and month.
            DateTime creationTime = GetDateTakenFromImage(path);
            int year = creationTime.Year;
            int month = creationTime.Month;
            //new path to output folder
            string yearPath = this.m_OutputFolder + "\\" + year.ToString();
            string monthPath = this.m_OutputFolder + "\\" + year.ToString() + "\\" + month.ToString();
            //new path to thumbnail folder
            string thumbnailYearPath = this.m_OutputFolder + "\\Thumbnails\\" + year.ToString();
            string thumbnailMonthPath = this.m_OutputFolder + "\\Thumbnails\\" + year.ToString() + "\\" + month.ToString();
            //create directories for new paths.
            Directory.CreateDirectory(yearPath);
            Directory.CreateDirectory(monthPath);
            Directory.CreateDirectory(thumbnailYearPath);
            Directory.CreateDirectory(thumbnailMonthPath);
            string[] paths = { monthPath, thumbnailMonthPath };
            return paths;
        }
        /*
         * param name = path for picture.
         * func returns when the pic was taken.
         */
        public static DateTime GetDateTakenFromImage(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (Image myImage = Image.FromStream(fs, false, false))

                {
                    PropertyItem propItem = myImage.GetPropertyItem(36867);
                    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                    return DateTime.Parse(dateTaken);
                }
            }
            catch
            {// fail to get time throw FAIL msg.
                return DateTime.Now;
            }
        }
    }
}