using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackApp.Models.Input
{
    public class ImageInputModel
    {
        public string Image { get; set; }
        public string Owner { get; set; }
    }
}
