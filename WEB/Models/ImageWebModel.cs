using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WEB.Communication;

namespace WEB.Models
{
    public class ImageWebModel
    {
        private string status;
        private int numOfPics;
        private List<Student> info = new List<Student>();
        private IImageServiceClient client;

        public ImageWebModel(PhotoListModel photoList)
        {
            client = ImageServiceClient.Instance;
            bool connected = client.IsConnected;
            numOfPics = photoList.Length();
            status = ConnectionStatus(connected);
            this.ParseInfo();
        }

        public string GetStatus { get { return this.status; } }
        public int GetNumofPics { get { return this.numOfPics; } }
        public List<Student> GetInfo { get { return this.info; } }

        private string ConnectionStatus(bool connected)
        {
            switch (connected)
            {
                case true:
                    return "connected";
                default:
                    return "disconnected";
            }
        }

        private void ParseInfo()
        {
            string text;
            string[] line;
            StreamReader file = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/details.txt"));
            text = file.ReadLine();
            while (text != null)
            {
                line = text.Split(' ');
                this.info.Add(new Student(line[0], line[1], line[2]));
                text = file.ReadLine();
            }
            file.Close();
        }
    }

    public class Student
    {
        private string firstName, lastName, ID;
        public Student(string first, string last, string id)
        {
            this.firstName = first;
            this.lastName = last;
            this.ID = id;
        }

        public string GetFirstName
        {
            get { return this.firstName; }
        }
        public string GetLastName
        {
            get { return this.lastName; }
        }
        public string GetID { get { return this.ID; } }
    }
}