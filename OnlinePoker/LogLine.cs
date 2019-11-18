using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsExam
{
    class LogLine
    {
        public string FilePath { get; set; }
        public int FileSize { get; set; }
        public int CountContains { get; set; }
        public override string ToString() => $"\"{FilePath}\" \"{FileSize}\" \"{CountContains}\"";
    }
}
