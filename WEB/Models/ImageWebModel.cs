using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Communication.Connection;

namespace WEB.Models
{
    //class has responsibility to show the students that made the project and the number of pics in dir.
    public class ImageWebModel
    {
        private static ImageWebModel instance;
        //singelton constructor implementation for client gui.
        public static ImageWebModel Instance
        {
            //singleton implementation
            get
            {
                if (instance == null)
                {
                    instance = new ImageWebModel();
                }
                return instance;
            }
        }
        private static IClientConnection Client { get; set; } //the connection to server.
        //constructor
        public ImageWebModel()
        {
            Client = ClientConnection.Instance;
            NumOfPics = 0;
            IsConnected = Client.IsConnected;
            Students = StudentsInit();
        }

        /*
         * initializing the list of students from AppData details file.
         * returns - the List of all students that made the project of Advenced Programming 2.
         */
        public static List<Student> StudentsInit()
        {
            List<Student> st = new List<Student>();
            StreamReader file = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Details.txt"));
            string line;
            while ((line = file.ReadLine()) != null)
            {
                string[] details = line.Split(' ');
                st.Add(new Student() { firstName = details[0], lastName = details[1], id = details[2] });
            }
            file.Close();
            return st;
        }
        //fields to show on web app.
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Students")]
        public List<Student> Students { get; set; }
        [Required]
        [Display(Name = "Is Connected")]
        public bool IsConnected { get; set; }
        [Required]
        [Display(Name = "Number of Pictures")]
        public int NumOfPics { get; set; }
        public class Student
        {
            [Required]
            [Display(Name = "First Name")]
            public string firstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string lastName { get; set; }

            [Required]
            [Display(Name = "ID")]
            public string id { get; set; }
        }
    }
}