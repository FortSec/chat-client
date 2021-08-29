using System;
using System.Collections.Generic;
using System.Text;

namespace ChatClient.Responses
{
    public class Status
    {
        public string Response { get; set; }
        public Status_Data Data { get; set; }
    }

    public class Status_Data
    {
        public bool Accepts_Clients { get; set; }
        public bool Accepts_Guests { get; set; }
        public bool Registration_Allowed { get; set; }
        public int Connected_Clients { get; set; }
        public string Status { get; set; }
    }

    public class APIPath
    {
        public string Response { get; set; }
        public APIPath_Data Data { get; set; }
    }

    public class APIPath_Data
    {
        public string Path { get; set; }
    }
}
