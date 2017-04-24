using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace conmonapi.Models
{
    public class Content
    {
        public string Id {
            get {
                if (FileNameParent == null || FileNameParent == FileName)
                {
                    return $"{Author}-{Type}-{FileName}";
                }
                
                return $"{Author}-{Type}-{FileNameParent}-{FileName}";
            }
        }
        public string Type { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public string Author { get; set; }

        public string FileNameParent { get; set; }
    }
}