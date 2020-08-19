using CommandLine;
using System;
using System.Collections.Generic;
using IO = System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiShow
{
    class Options
    {
        [Option('p', "path", Required = false, HelpText = "Path to file or folder. If not specified, current directory is used.")]
        public string Path { get; set; }

        public string GetPath()
        {
            string ret = string.IsNullOrEmpty(Path) ? Environment.CurrentDirectory : Path;
            return IO.Path.GetFullPath(ret);
        }
    }
}
