using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBooter.Models
{
    class Downloader
    {
        const string TEMP_EXT = "tmp";
        const int BUFF_SIZE = 256;
        public string URL { get; private set; }
        public int Size { get; private set; }
        public string PathToSave { get; private set; }
        public Downloader(string url, string pathToSave)
        {
            URL = url;
            PathToSave = pathToSave;
        }
    }
}
