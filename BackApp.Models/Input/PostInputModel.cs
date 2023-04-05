using System;
using System.Collections.Generic;
using System.Text;

namespace BackApp.Models.Input
{
    public class PostInputModel
    {
        public string User { get; set; }
        public string Url { get; set; }
        public string Post { get; set; }
        public DateTime Date { get; set; }
    }
}
