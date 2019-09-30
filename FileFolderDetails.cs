using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace explorer_wsinf
{
    class FileFolderDetails
    {
        public string FileName { get; set; }
        public string FileImage { get; set; }
        public string FileCreation { get; set; }
        public string Path { get; set; }

        public bool IsFile = false;
        public bool IsFolder = false;
    }
}
