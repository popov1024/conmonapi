using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace conmonapi.Models
{
    public class Content
    {
        private string fileNameChilde;

        public Content(string author, string type, string fileName, string fileNameChilde)
        {
            Author = author;
            Type = type;
            FileName = fileName;
            this.fileNameChilde = fileNameChilde;
        }

        public Content(string author, string type, string fileName, string fileNameChilde, string contentType)
        {
            FileName = fileNameChilde == null ? fileName : fileNameChilde;
            ContentType = contentType;
            Author = author;
            FileNameParent = fileName;
            Type = type;
        }

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