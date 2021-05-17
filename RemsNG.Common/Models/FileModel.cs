using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RemsNG.Common.Models
{
    public class FileModel
    {
        /// <summary>
        /// Desired name for file e.g. "Passport Photo"
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Full File Path e.g. "path/to/file/file.jpg"
        /// </summary>
        public string Path { get; }
        /// <summary>
        /// Returns just the Filename e.g. "file.jpg"
        /// </summary>
        public string FileName => System.IO.Path.GetFileName(Path);

        /// <summary>
        /// File Extension e.g. ".jpg"
        /// </summary>
        public string Extension { get; }

        /// <summary>
        /// Mime type of File e.g. "image/jpeg"
        /// </summary>
        public string MimeType { get; }

        /// <summary>
        /// Raw File Contents
        /// </summary>
        public Stream Content { get; }

        /// <summary>
        /// Creates a Readonly File Object with Name, Path, Extension and Mime Type
        /// </summary>
        /// <param name="name">Desired name of file e.g. First File</param>
        /// <param name="path">Full Path/Filename e.g. "path/to/file.ext"</param>
        /// <param name="content">Raw file/memory stream</param>
        /// 
        public string StringContent { get; set; }
        public FileModel(string name, string path, Stream content)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("Invalid Name of File");
            Name = name;
            Path = path;
            Content = content;
            (Extension, MimeType) = GetMimeType(path);

        }

        private (string, string) GetMimeType(string path)
        {
            var file = path.ToLower();
            var map = new Dictionary<string, string>
            {
                {".csv", "text/csv"},
                {".jpeg", "image/jpeg"},
                {".jpg", "image/jpeg"},
                {".png", "image/png"},
                {".pdf", "application/pdf"},
                {".doc", "application/msword"},
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".txt", "text/plain"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".ppt", "application/vnd.ms-powerpoint"},
                {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
            };

            var query = from mime in map
                        where file.EndsWith(mime.Key, StringComparison.OrdinalIgnoreCase)
                        select (mime.Key, mime.Value);

            if (!query.Any()) throw new Exception("Could not find Mime Type");

            return query.First();
        }

        public bool IsExcel
        {
            get
            {
                if (this.Path.ToLower().EndsWith(".xlsx") || this.Path.ToLower().EndsWith("xls"))
                    return true;
                return false;
            }
        }
        public bool IsImage
        {
            get
            {
                if (this.Path.ToLower().EndsWith(".jpg")
                    || this.Path.ToLower().EndsWith(".jpeg")
                    || this.Path.ToLower().EndsWith(".png")
                    || this.Path.ToLower().EndsWith(".pdf"))
                    return true;
                return false;
            }
        }

    }
}
